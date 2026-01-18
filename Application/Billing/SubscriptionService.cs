using Contracts.Billing;
using Contracts.Members;
using Domain.Billing;
using Infrastructure;
using Infrastructure.Billing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public sealed class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subsRepo;
        private readonly NextGymDbContext _db; // used for member/package existence checks & receipt data

        public SubscriptionService(ISubscriptionRepository subsRepo, NextGymDbContext db)
        {
            _subsRepo = subsRepo;
            _db = db;
        }

        public async Task<PagedResponse<SubscriptionListItemDto>> GetPagedAsync(long? memberId, string? status, int page, int pageSize, CancellationToken ct)
        {
            var (items, total) = await _subsRepo.GetPagedAsync(memberId, status, page, pageSize, ct);

            // compute paid/outstanding per item (simple but accurate)
            var list = new List<SubscriptionListItemDto>(items.Count);
            foreach (var s in items)
            {
                var paid = await _subsRepo.GetTotalPaidAsync(s.Id, ct);
                var outstanding = Math.Max(s.TotalPayable - paid, 0);

                var days = (int)(s.EndDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).TotalDays;
                list.Add(new SubscriptionListItemDto(
                    s.Id,
                    s.MemberId,
                    s.PackageId,
                    s.Package.Name,
                    s.StartDate,
                    s.EndDate,
                    s.Status.ToString(),
                    s.TotalPayable,
                    paid,
                    outstanding,
                    days
                ));
            }

            return new PagedResponse<SubscriptionListItemDto>(list, page, pageSize, total);
        }

        public async Task<SubscriptionDetailDto?> GetByIdAsync(long id, CancellationToken ct)
        {
            var s = await _subsRepo.GetByIdWithPaymentsAsync(id, ct);
            if (s is null) return null;

            var paid = s.Payments.Where(p => p.Status == PaymentStatus.PAID).Sum(p => p.Amount);
            var outstanding = Math.Max(s.TotalPayable - paid, 0);

            var payments = s.Payments
                .OrderByDescending(p => p.PaidAt)
                .ThenByDescending(p => p.Id)
                .Select(p => new PaymentDto(p.Id, p.SubscriptionId, p.PaidAt, p.Method.ToString(), p.Amount, p.Reference, p.Status.ToString()))
                .ToList();

            return new SubscriptionDetailDto(
                s.Id,
                s.MemberId,
                s.PackageId,
                s.Package.Name,
                s.Package.DurationDays,
                s.StartDate,
                s.EndDate,
                s.Status.ToString(),
                s.Amount,
                s.Discount,
                s.Taxes,
                s.TotalPayable,
                paid,
                outstanding,
                s.Notes,
                payments
            );
        }

        public async Task<SubscriptionDetailDto> CreateAsync(CreateSubscriptionRequest req, CancellationToken ct)
        {
            // Validate member/package exist
            var memberExists = await _db.Members.AsNoTracking().AnyAsync(m => m.Id == req.MemberId, ct);
            if (!memberExists) throw new InvalidOperationException("Member not found.");

            var package = await _db.Packages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == req.PackageId, ct);
            if (package is null) throw new InvalidOperationException("Package not found.");
            if (!package.IsActive) throw new InvalidOperationException("Package is inactive.");

            var start = req.StartDate ?? DateOnly.FromDateTime(DateTime.Today);

            // EndDate/Amount can be computed by DB triggers (your schema), but we set good defaults too:
            var end = req.EndDate ?? start.AddDays(package.DurationDays - 1);
            var amount = req.Amount ?? package.Price;
            var discount = req.Discount ?? 0m;
            var taxes = req.Taxes ?? 0m;
            var total = Math.Max(amount - discount + taxes, 0);

            var sub = new Subscription
            {
                MemberId = req.MemberId,
                PackageId = req.PackageId,
                StartDate = start,
                EndDate = end,
                Status = SubscriptionStatus.ACTIVE,
                Amount = amount,
                Discount = discount,
                Taxes = taxes,
                TotalPayable = total,
                Notes = string.IsNullOrWhiteSpace(req.Notes) ? null : req.Notes.Trim()
            };

            try
            {
                await _subsRepo.AddAsync(sub, ct);
            }
            catch (DbUpdateException ex)
            {
                // If your MySQL trigger blocks overlap, you'll land here
                throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
            }

            return (await GetByIdAsync(sub.Id, ct))!;
        }

        public async Task<SubscriptionDetailDto?> UpdateStatusAsync(long id, UpdateSubscriptionStatusRequest req, CancellationToken ct)
        {
            var s = await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (s is null) return null;

            if (!Enum.TryParse<SubscriptionStatus>(req.Status.Trim(), true, out var st))
                throw new InvalidOperationException("Invalid status. Use ACTIVE / PAUSED / EXPIRED.");

            s.Status = st;
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }

        public async Task<MemberDuesDto?> GetMemberDuesAsync(long memberId, CancellationToken ct)
        {
            var memberExists = await _db.Members.AsNoTracking().AnyAsync(m => m.Id == memberId, ct);
            if (!memberExists) return null;

            var totals = await _subsRepo.GetMemberTotalsAsync(memberId, ct);
            if (totals is null) return new MemberDuesDto(memberId, 0, 0, 0);

            var outstanding = Math.Max(totals.TotalPayable - totals.TotalPaid, 0);
            return new MemberDuesDto(memberId, totals.TotalPayable, totals.TotalPaid, outstanding);
        }
    }
}

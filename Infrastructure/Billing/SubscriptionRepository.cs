using Domain.Billing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public sealed class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NextGymDbContext _db;
        public SubscriptionRepository(NextGymDbContext db) => _db = db;

        public Task<Subscription?> GetByIdWithPaymentsAsync(long id, CancellationToken ct) =>
            _db.Subscriptions
                .Include(s => s.Package)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<(IReadOnlyList<Subscription> Items, int Total)> GetPagedAsync(
            long? memberId, string? status, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

            IQueryable<Subscription> q = _db.Subscriptions.AsNoTracking();

            if (memberId.HasValue)
                q = q.Where(s => s.MemberId == memberId.Value);

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<SubscriptionStatus>(status.Trim(), true, out var st))
                q = q.Where(s => s.Status == st);

            var total = await q.CountAsync(ct);

            var items = await q
                .Include(s => s.Package)   // include AFTER filtering (also more efficient)
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }


        public async Task AddAsync(Subscription entity, CancellationToken ct)
        {
            _db.Subscriptions.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Subscription entity, CancellationToken ct)
        {
            _db.Subscriptions.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<decimal> GetTotalPaidAsync(long subscriptionId, CancellationToken ct)
        {
            return await _db.Payments.AsNoTracking()
                .Where(p => p.SubscriptionId == subscriptionId && p.Status == PaymentStatus.PAID)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;
        }

        public async Task<MemberSubscriptionTotals?> GetMemberTotalsAsync(long memberId, CancellationToken ct)
        {
            var subs = await _db.Subscriptions.AsNoTracking()
                .Where(s => s.MemberId == memberId)
                .Select(s => new { s.Id, s.TotalPayable })
                .ToListAsync(ct);

            if (subs.Count == 0) return null;

            var totalPayable = subs.Sum(x => x.TotalPayable);

            var subIds = subs.Select(x => x.Id).ToList();

            var totalPaid = await _db.Payments.AsNoTracking()
                .Where(p => subIds.Contains(p.SubscriptionId) && p.Status == PaymentStatus.PAID)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

            return new MemberSubscriptionTotals(totalPayable, totalPaid);
        }
    }
}

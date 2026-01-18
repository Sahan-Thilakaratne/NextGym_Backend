using Contracts.Billing;
using Domain.Billing;
using Infrastructure;
using Infrastructure.Billing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _payRepo;
        private readonly ISubscriptionRepository _subRepo;
        private readonly NextGymDbContext _db;

        public PaymentService(IPaymentRepository payRepo, ISubscriptionRepository subRepo, NextGymDbContext db)
        {
            _payRepo = payRepo;
            _subRepo = subRepo;
            _db = db;
        }

        public async Task<PaymentDto> RecordAsync(RecordPaymentRequest req, CancellationToken ct)
        {
            if (!Enum.TryParse<PaymentMethod>(req.Method.Trim(), true, out var method))
                throw new InvalidOperationException("Invalid method. Use CASH/CARD/ONLINE.");

            var status = PaymentStatus.PAID;
            if (!string.IsNullOrWhiteSpace(req.Status))
            {
                if (!Enum.TryParse<PaymentStatus>(req.Status.Trim(), true, out status))
                    throw new InvalidOperationException("Invalid status. Use PAID/FAILED/REFUNDED.");
            }

            // validate subscription exists
            var subExists = await _db.Subscriptions.AsNoTracking().AnyAsync(s => s.Id == req.SubscriptionId, ct);
            if (!subExists) throw new InvalidOperationException("Subscription not found.");

            if (req.Amount <= 0) throw new InvalidOperationException("Amount must be > 0.");

            var payment = new Payment
            {
                SubscriptionId = req.SubscriptionId,
                PaidAt = req.PaidAt ?? DateTime.UtcNow,
                Method = method,
                Amount = req.Amount,
                Reference = string.IsNullOrWhiteSpace(req.Reference) ? null : req.Reference.Trim(),
                Status = status
            };

            await _payRepo.AddAsync(payment, ct);

            return new PaymentDto(payment.Id, payment.SubscriptionId, payment.PaidAt, payment.Method.ToString(), payment.Amount, payment.Reference, payment.Status.ToString());
        }

        public async Task<IReadOnlyList<PaymentDto>> GetBySubscriptionAsync(long subscriptionId, CancellationToken ct)
        {
            var subExists = await _db.Subscriptions.AsNoTracking().AnyAsync(s => s.Id == subscriptionId, ct);
            if (!subExists) return Array.Empty<PaymentDto>();

            var items = await _payRepo.GetBySubscriptionAsync(subscriptionId, ct);

            return items.Select(p => new PaymentDto(
                p.Id, p.SubscriptionId, p.PaidAt, p.Method.ToString(), p.Amount, p.Reference, p.Status.ToString()
            )).ToList();
        }

        public async Task<ReceiptDto?> GetReceiptAsync(long paymentId, CancellationToken ct)
        {
            var payment = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == paymentId, ct);
            if (payment is null) return null;

            var sub = await _db.Subscriptions.AsNoTracking()
                .Include(s => s.Package)
                .Include(s => s.Member)
                .FirstOrDefaultAsync(s => s.Id == payment.SubscriptionId, ct);

            if (sub is null) return null;

            var paid = await _subRepo.GetTotalPaidAsync(sub.Id, ct);
            var outstanding = Math.Max(sub.TotalPayable - paid, 0);

            var memberName = $"{sub.Member.FirstName} {sub.Member.LastName}".Trim();

            return new ReceiptDto(
                payment.Id,
                payment.PaidAt,
                payment.Amount,
                payment.Method.ToString(),
                payment.Reference,
                sub.Id,
                sub.MemberId,
                sub.Member.MemberCode,
                memberName,
                sub.Package.Name,
                sub.StartDate,
                sub.EndDate,
                sub.TotalPayable,
                paid,
                outstanding
            );
        }
    }
}

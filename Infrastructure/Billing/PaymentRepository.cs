using Domain.Billing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly NextGymDbContext _db;
        public PaymentRepository(NextGymDbContext db) => _db = db;

        public Task<Payment?> GetByIdAsync(long id, CancellationToken ct) =>
            _db.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IReadOnlyList<Payment>> GetBySubscriptionAsync(long subscriptionId, CancellationToken ct)
        {
            return await _db.Payments.AsNoTracking()
                .Where(p => p.SubscriptionId == subscriptionId)
                .OrderByDescending(p => p.PaidAt)
                .ThenByDescending(p => p.Id)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Payment payment, CancellationToken ct)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync(ct);
        }
    }
}

using Domain.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(long id, CancellationToken ct);
        Task<IReadOnlyList<Payment>> GetBySubscriptionAsync(long subscriptionId, CancellationToken ct);
        Task AddAsync(Payment payment, CancellationToken ct);
    }
}

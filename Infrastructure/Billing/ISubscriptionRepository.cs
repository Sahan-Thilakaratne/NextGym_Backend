using Domain.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdWithPaymentsAsync(long id, CancellationToken ct);
        Task<(IReadOnlyList<Subscription> Items, int Total)> GetPagedAsync(long? memberId, string? status, int page, int pageSize, CancellationToken ct);

        Task AddAsync(Subscription entity, CancellationToken ct);
        Task UpdateAsync(Subscription entity, CancellationToken ct);

        Task<decimal> GetTotalPaidAsync(long subscriptionId, CancellationToken ct);
        Task<MemberSubscriptionTotals?> GetMemberTotalsAsync(long memberId, CancellationToken ct);
    }

    public sealed record MemberSubscriptionTotals(decimal TotalPayable, decimal TotalPaid);
}

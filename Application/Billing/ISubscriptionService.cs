using Contracts.Billing;
using Contracts.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public interface ISubscriptionService
    {
        Task<PagedResponse<SubscriptionListItemDto>> GetPagedAsync(long? memberId, string? status, int page, int pageSize, CancellationToken ct);
        Task<SubscriptionDetailDto?> GetByIdAsync(long id, CancellationToken ct);

        Task<SubscriptionDetailDto> CreateAsync(CreateSubscriptionRequest req, CancellationToken ct); // new / renew
        Task<SubscriptionDetailDto?> UpdateStatusAsync(long id, UpdateSubscriptionStatusRequest req, CancellationToken ct); // pause/resume/expire

        Task<MemberDuesDto?> GetMemberDuesAsync(long memberId, CancellationToken ct);
    }
}

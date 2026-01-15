using Contracts.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Members
{
    public interface IMemberService
    {

        Task<PagedResponse<MemberListItemDto>> GetMembersAsync(GetMembersQuery q, CancellationToken ct);
        Task<MemberDetailDto?> GetMemberByIdAsync(long id, CancellationToken ct);
        Task<MemberDetailDto> CreateMemberAsync(CreateMemberRequest req, CancellationToken ct);
        Task<MemberDetailDto?> UpdateMemberAsync(long id, UpdateMemberRequest req, CancellationToken ct);
        Task<bool> DeleteMemberAsync(long id, CancellationToken ct);

        Task<HealthProfileDto?> GetHealthProfileAsync(long memberId, CancellationToken ct);
        Task<HealthProfileDto?> UpsertHealthProfileAsync(long memberId, UpsertHealthProfileRequest req, CancellationToken ct);

        Task<List<WeightLogDto>?> GetWeightLogsAsync(long memberId, CancellationToken ct);
        Task<WeightLogDto?> AddWeightLogAsync(long memberId, AddWeightLogRequest req, CancellationToken ct);
        Task<bool> DeleteWeightLogAsync(long memberId, long logId, CancellationToken ct);

        Task<BmiLatestDto?> GetLatestBmiAsync(long memberId, CancellationToken ct);
    }
}

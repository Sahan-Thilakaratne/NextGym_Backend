using Domain.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Members
{
    public interface IMemberRepository
    {
        Task<(IReadOnlyList<Member> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken ct);
        Task<Member?> GetByIdAsync(long id, CancellationToken ct);
        Task<Member?> GetByIdWithHealthAsync(long id, CancellationToken ct);

        Task AddAsync(Member member, CancellationToken ct);
        Task UpdateAsync(Member member, CancellationToken ct);
        Task DeleteAsync(Member member, CancellationToken ct);

        Task<HealthProfile?> GetHealthAsync(long memberId, CancellationToken ct);
        Task UpsertHealthAsync(HealthProfile profile, CancellationToken ct);

        Task<IReadOnlyList<WeightLog>?> GetWeightsAsync(long memberId, CancellationToken ct);
        Task AddWeightAsync(WeightLog log, CancellationToken ct);
        Task<WeightLog?> GetWeightByIdAsync(long memberId, long logId, CancellationToken ct);
        Task DeleteWeightAsync(WeightLog log, CancellationToken ct);

        Task<bool> MemberExistsAsync(long memberId, CancellationToken ct);
        Task<bool> MemberCodeExistsAsync(string code, CancellationToken ct);
    }
}

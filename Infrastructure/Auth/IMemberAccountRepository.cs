using Domain.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public interface IMemberAccountRepository
    {
        Task<MemberAccount?> GetByIdAsync(long id, CancellationToken ct);
        Task<MemberAccount?> GetByMemberIdAsync(long memberId, CancellationToken ct);

        Task<bool> UsernameExistsAsync(string username, long? excludeId, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, long? excludeId, CancellationToken ct);

        Task AddAsync(MemberAccount entity, CancellationToken ct);
        Task UpdateAsync(MemberAccount entity, CancellationToken ct);
        Task DeleteAsync(MemberAccount entity, CancellationToken ct);
    }
}

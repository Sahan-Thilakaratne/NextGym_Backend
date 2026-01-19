using Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth
{
    public interface IMemberAccountService
    {
        Task<MemberAccountDto?> GetByMemberIdAsync(long memberId, CancellationToken ct);
        Task<MemberAccountDto> CreateAsync(CreateMemberAccountRequest req, CancellationToken ct);
        Task<MemberAccountDto?> UpdateAsync(long memberId, UpdateMemberAccountRequest req, CancellationToken ct);
        Task<bool> DeleteAsync(long memberId, CancellationToken ct);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Auth
{
    public sealed record MemberAccountDto(
    long Id,
    long MemberId,
    string? Username,
    string? Email,
    bool IsActive,
    DateTime? LastLoginAt
);

    public sealed record CreateMemberAccountRequest(
        long MemberId,
        string? Username,
        string? Email,
        string Password
    );

    public sealed record UpdateMemberAccountRequest(
        string? Username,
        string? Email,
        string? Password,
        bool? IsActive
    );
}

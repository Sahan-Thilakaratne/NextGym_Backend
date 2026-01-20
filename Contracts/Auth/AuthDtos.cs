using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Auth
{
    public sealed record LoginRequest(string Username, string Password);

    public sealed record AuthUserDto(
        long Id,
        string Username,
        string Name,
        string Role
    );

    public sealed record LoginResponse(
        string AccessToken,
        DateTime ExpiresAtUtc,
        AuthUserDto User
    );

    public sealed record RegisterUserRequest(
    string Username,
    string Name,
    string Password,
    string Role,
    string? Mobile,
    string? Email
);

    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );

    public sealed record MeResponse(
        long Id,
        string Username,
        string Name,
        string Role,
        bool IsActive
    );
}

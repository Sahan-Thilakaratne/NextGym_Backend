using Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct);

        Task<MeResponse> GetMeAsync(long userId, CancellationToken ct);

        Task<MeResponse> RegisterAsync(RegisterUserRequest req, CancellationToken ct);

        Task ChangePasswordAsync(long userId, ChangePasswordRequest req, CancellationToken ct);
    }
}

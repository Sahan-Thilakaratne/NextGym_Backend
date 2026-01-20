using Contracts.Auth;
using Domain.Users;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokens;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(IUserRepository users, ITokenService tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            var username = (req.Username ?? "").Trim();
            var password = req.Password ?? "";

            var user = await _users.GetByUsernameAsync(username, ct);
            if (user is null || !user.IsActive)
                throw new InvalidOperationException("Invalid credentials.");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid credentials.");

            var (token, exp) = _tokens.CreateToken(user);

            return new LoginResponse(
                token,
                exp,
                new AuthUserDto(user.Id, user.Username, user.Name, user.Role.ToString())
            );
        }

        public async Task<MeResponse> GetMeAsync(long userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);
            if (user is null) throw new InvalidOperationException("User not found.");
            return new MeResponse(user.Id, user.Username, user.Name, user.Role.ToString(), user.IsActive);
        }

        public async Task<MeResponse> RegisterAsync(RegisterUserRequest req, CancellationToken ct)
        {
            var username = (req.Username ?? "").Trim();
            var name = (req.Name ?? "").Trim();
            var password = req.Password ?? "";

            if (string.IsNullOrWhiteSpace(username)) throw new InvalidOperationException("Username is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Name is required.");
            if (password.Trim().Length < 6) throw new InvalidOperationException("Password must be at least 6 characters.");

            if (await _users.UsernameExistsAsync(username, null, ct))
                throw new InvalidOperationException("Username already exists.");

            if (!Enum.TryParse<UserRole>((req.Role ?? "").Trim(), true, out var role))
                throw new InvalidOperationException("Invalid role.");

            var user = new User
            {
                Username = username,
                Name = name,
                Role = role,
                Mobile = string.IsNullOrWhiteSpace(req.Mobile) ? null : req.Mobile.Trim(),
                Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim(),
                IsActive = true
            };

            user.PasswordHash = _hasher.HashPassword(user, password.Trim());

            await _users.AddAsync(user, ct);

            return new MeResponse(user.Id, user.Username, user.Name, user.Role.ToString(), user.IsActive);
        }

        public async Task ChangePasswordAsync(long userId, ChangePasswordRequest req, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);
            if (user is null) throw new InvalidOperationException("User not found.");

            var current = req.CurrentPassword ?? "";
            var next = req.NewPassword ?? "";

            if (next.Trim().Length < 6) throw new InvalidOperationException("New password must be at least 6 characters.");

            var check = _hasher.VerifyHashedPassword(user, user.PasswordHash, current);
            if (check == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = _hasher.HashPassword(user, next.Trim());
            await _users.UpdateAsync(user, ct);
        }
    }
}

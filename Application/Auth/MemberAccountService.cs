using Contracts.Auth;
using Domain.Auth;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Application.Auth
{
    public sealed class MemberAccountService : IMemberAccountService
    {
        private readonly IMemberAccountRepository _repo;
        private readonly NextGymDbContext _db;
        private readonly PasswordHasher<MemberAccount> _hasher = new();

        public MemberAccountService(IMemberAccountRepository repo, NextGymDbContext db)
        {
            _repo = repo;
            _db = db;  
        }

        public async Task<MemberAccountDto?> GetByMemberIdAsync(long memberId, CancellationToken ct)
        {
            var acc = await _repo.GetByMemberIdAsync(memberId, ct);
            return acc is null ? null : ToDto(acc);
        }

        public async Task<MemberAccountDto> CreateAsync(CreateMemberAccountRequest req, CancellationToken ct)
        {
            // member must exist
            var memberExists = await _db.Members.AsNoTracking().AnyAsync(m => m.Id == req.MemberId, ct);
            if (!memberExists) throw new InvalidOperationException("Member not found.");

            // one account per member
            var existing = await _repo.GetByMemberIdAsync(req.MemberId, ct);
            if (existing is not null) throw new InvalidOperationException("Member already has an account.");

            var username = string.IsNullOrWhiteSpace(req.Username) ? null : req.Username.Trim();
            var email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim();

            if (username is null && email is null)
                throw new InvalidOperationException("Username or Email is required.");

            if (username is not null && await _repo.UsernameExistsAsync(username, null, ct))
                throw new InvalidOperationException("Username already exists.");

            if (email is not null && await _repo.EmailExistsAsync(email, null, ct))
                throw new InvalidOperationException("Email already exists.");

            if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Trim().Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters.");

            var entity = new MemberAccount
            {
                MemberId = req.MemberId,
                Username = username,
                Email = email,
                IsActive = true
            };

            entity.PasswordHash = _hasher.HashPassword(entity, req.Password.Trim());

            await _repo.AddAsync(entity, ct);
            return ToDto(entity);
        }

        public async Task<MemberAccountDto?> UpdateAsync(long memberId, UpdateMemberAccountRequest req, CancellationToken ct)
        {
            var acc = await _repo.GetByMemberIdAsync(memberId, ct);
            if (acc is null) return null;

            var username = string.IsNullOrWhiteSpace(req.Username) ? null : req.Username.Trim();
            var email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim();

            if (username is not null && await _repo.UsernameExistsAsync(username, acc.Id, ct))
                throw new InvalidOperationException("Username already exists.");

            if (email is not null && await _repo.EmailExistsAsync(email, acc.Id, ct))
                throw new InvalidOperationException("Email already exists.");

            acc.Username = username ?? acc.Username;
            acc.Email = email ?? acc.Email;

            if (req.IsActive.HasValue) acc.IsActive = req.IsActive.Value;

            if (!string.IsNullOrWhiteSpace(req.Password))
            {
                var pw = req.Password.Trim();
                if (pw.Length < 6) throw new InvalidOperationException("Password must be at least 6 characters.");
                acc.PasswordHash = _hasher.HashPassword(acc, pw);
            }

            await _repo.UpdateAsync(acc, ct);
            return ToDto(acc);
        }

        public async Task<bool> DeleteAsync(long memberId, CancellationToken ct)
        {
            var acc = await _repo.GetByMemberIdAsync(memberId, ct);
            if (acc is null) return false;

            await _repo.DeleteAsync(acc, ct);
            return true;
        }

        private static MemberAccountDto ToDto(MemberAccount a) =>
            new(a.Id, a.MemberId, a.Username, a.Email, a.IsActive, a.LastLoginAt);
    }
}

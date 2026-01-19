using Domain.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public sealed class MemberAccountRepository : IMemberAccountRepository
    {
        private readonly NextGymDbContext _db;
        public MemberAccountRepository(NextGymDbContext db) => _db = db;

        public Task<MemberAccount?> GetByIdAsync(long id, CancellationToken ct) =>
            _db.MemberAccounts.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<MemberAccount?> GetByMemberIdAsync(long memberId, CancellationToken ct) =>
            _db.MemberAccounts.FirstOrDefaultAsync(x => x.MemberId == memberId, ct);

        public Task<bool> UsernameExistsAsync(string username, long? excludeId, CancellationToken ct)
        {
            var q = _db.MemberAccounts.AsNoTracking()
                .Where(x => x.Username != null && x.Username == username);
            if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
            return q.AnyAsync(ct);
        }

        public Task<bool> EmailExistsAsync(string email, long? excludeId, CancellationToken ct)
        {
            var q = _db.MemberAccounts.AsNoTracking()
                .Where(x => x.Email != null && x.Email == email);
            if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
            return q.AnyAsync(ct);
        }

        public async Task AddAsync(MemberAccount entity, CancellationToken ct)
        {
            _db.MemberAccounts.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(MemberAccount entity, CancellationToken ct)
        {
            _db.MemberAccounts.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(MemberAccount entity, CancellationToken ct)
        {
            _db.MemberAccounts.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

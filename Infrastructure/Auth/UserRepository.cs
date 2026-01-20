using Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly NextGymDbContext _db;
        public UserRepository(NextGymDbContext db) => _db = db;

        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct) =>
            _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

        public Task<User?> GetByIdAsync(long id, CancellationToken ct) =>
            _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<bool> UsernameExistsAsync(string username, long? excludeId, CancellationToken ct)
        {
            var q = _db.Users.AsNoTracking().Where(u => u.Username == username);
            if (excludeId.HasValue) q = q.Where(u => u.Id != excludeId.Value);
            return q.AnyAsync(ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}

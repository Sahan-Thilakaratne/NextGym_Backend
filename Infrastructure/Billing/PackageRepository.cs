using Domain.Billing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public sealed class PackageRepository : IPackageRepository
    {
        private readonly NextGymDbContext _db;
        public PackageRepository(NextGymDbContext db) => _db = db;

        public async Task<IReadOnlyList<Package>> GetAllAsync(bool? onlyActive, CancellationToken ct)
        {
            var q = _db.Packages.AsNoTracking();

            if (onlyActive == true)
                q = q.Where(p => p.IsActive);

            return await q.OrderBy(p => p.Name).ToListAsync(ct);
        }

        public Task<Package?> GetByIdAsync(long id, CancellationToken ct) =>
            _db.Packages.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task AddAsync(Package entity, CancellationToken ct)
        {
            _db.Packages.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Package entity, CancellationToken ct)
        {
            _db.Packages.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Package entity, CancellationToken ct)
        {
            _db.Packages.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> NameExistsAsync(string name, long? excludeId, CancellationToken ct)
        {
            var q = _db.Packages.AsNoTracking().Where(p => p.Name == name);
            if (excludeId.HasValue) q = q.Where(p => p.Id != excludeId.Value);
            return q.AnyAsync(ct);
        }
    }
}

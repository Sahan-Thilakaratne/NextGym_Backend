using Domain.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Billing
{
    public interface IPackageRepository
    {
        Task<IReadOnlyList<Package>> GetAllAsync(bool? onlyActive, CancellationToken ct);
        Task<Package?> GetByIdAsync(long id, CancellationToken ct);
        Task AddAsync(Package entity, CancellationToken ct);
        Task UpdateAsync(Package entity, CancellationToken ct);
        Task DeleteAsync(Package entity, CancellationToken ct);
        Task<bool> NameExistsAsync(string name, long? excludeId, CancellationToken ct);
    }
}

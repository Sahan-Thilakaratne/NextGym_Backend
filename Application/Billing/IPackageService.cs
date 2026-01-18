using Contracts.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public interface IPackageService
    {
        Task<IReadOnlyList<PackageDto>> GetAllAsync(bool? onlyActive, CancellationToken ct);
        Task<PackageDto?> GetByIdAsync(long id, CancellationToken ct);
        Task<PackageDto> CreateAsync(CreatePackageRequest req, CancellationToken ct);
        Task<PackageDto?> UpdateAsync(long id, UpdatePackageRequest req, CancellationToken ct);
        Task<bool> DeleteAsync(long id, CancellationToken ct);
    }
}

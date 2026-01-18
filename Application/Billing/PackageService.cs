using Contracts.Billing;
using Domain.Billing;
using Infrastructure.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public sealed class PackageService : IPackageService
    {
        private readonly IPackageRepository _repo;

        public PackageService(IPackageRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<PackageDto>> GetAllAsync(bool? onlyActive, CancellationToken ct)
        {
            var items = await _repo.GetAllAsync(onlyActive, ct);
            return items.Select(ToDto).ToList();
        }

        public async Task<PackageDto?> GetByIdAsync(long id, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            return p is null ? null : ToDto(p);
        }

        public async Task<PackageDto> CreateAsync(CreatePackageRequest req, CancellationToken ct)
        {
            req = req with { Name = req.Name.Trim() };

            if (await _repo.NameExistsAsync(req.Name, null, ct))
                throw new InvalidOperationException("Package name already exists.");

            if (req.DurationDays < 1) throw new InvalidOperationException("DurationDays must be >= 1.");
            if (req.Price < 0) throw new InvalidOperationException("Price must be >= 0.");

            var entity = new Package
            {
                Name = req.Name,
                Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
                DurationDays = req.DurationDays,
                Price = req.Price,
                SessionLimit = req.SessionLimit,
                IsActive = req.IsActive ?? true
            };

            await _repo.AddAsync(entity, ct);
            return ToDto(entity);
        }

        public async Task<PackageDto?> UpdateAsync(long id, UpdatePackageRequest req, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing is null) return null;

            req = req with { Name = req.Name.Trim() };

            if (await _repo.NameExistsAsync(req.Name, id, ct))
                throw new InvalidOperationException("Package name already exists.");

            if (req.DurationDays < 1) throw new InvalidOperationException("DurationDays must be >= 1.");
            if (req.Price < 0) throw new InvalidOperationException("Price must be >= 0.");

            existing.Name = req.Name;
            existing.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
            existing.DurationDays = req.DurationDays;
            existing.Price = req.Price;
            existing.SessionLimit = req.SessionLimit;
            existing.IsActive = req.IsActive ?? existing.IsActive;

            await _repo.UpdateAsync(existing, ct);
            return ToDto(existing);
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing is null) return false;

            await _repo.DeleteAsync(existing, ct);
            return true;
        }

        private static PackageDto ToDto(Package p) =>
            new(p.Id, p.Name, p.Description, p.DurationDays, p.Price, p.SessionLimit, p.IsActive);
    }
}

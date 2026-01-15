using Domain.Members;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Members
{
    public sealed class MemberRepository : IMemberRepository
    {
        private readonly NextGymDbContext _db;

        public MemberRepository(NextGymDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Member> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken ct)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

            var q = _db.Members.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(m =>
                    m.MemberCode.Contains(s) ||
                    m.FirstName.Contains(s) ||
                    (m.LastName != null && m.LastName.Contains(s)) ||
                    m.Mobile.Contains(s) ||
                    (m.Email != null && m.Email.Contains(s))
                );
            }

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<Member?> GetByIdAsync(long id, CancellationToken ct) =>
            _db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

        public Task<Member?> GetByIdWithHealthAsync(long id, CancellationToken ct) =>
            _db.Members.AsNoTracking()
                .Include(m => m.HealthProfile)
                .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task AddAsync(Member member, CancellationToken ct)
        {
            _db.Members.Add(member);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Member member, CancellationToken ct)
        {
            _db.Members.Update(member);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Member member, CancellationToken ct)
        {
            _db.Members.Remove(member);
            await _db.SaveChangesAsync(ct);
        }

        public Task<HealthProfile?> GetHealthAsync(long memberId, CancellationToken ct) =>
            _db.HealthProfiles.AsNoTracking().FirstOrDefaultAsync(h => h.MemberId == memberId, ct);

        public async Task UpsertHealthAsync(HealthProfile profile, CancellationToken ct)
        {
            // if profile exists, update; else insert
            var existing = await _db.HealthProfiles.FirstOrDefaultAsync(h => h.MemberId == profile.MemberId, ct);
            if (existing is null)
            {
                _db.HealthProfiles.Add(profile);
            }
            else
            {
                existing.HeightCm = profile.HeightCm;
                existing.RestingHr = profile.RestingHr;
                existing.BloodPressure = profile.BloodPressure;
                existing.ConditionsJson = profile.ConditionsJson;
                existing.Notes = profile.Notes;
                _db.HealthProfiles.Update(existing);
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<WeightLog>?> GetWeightsAsync(long memberId, CancellationToken ct)
        {
            var exists = await MemberExistsAsync(memberId, ct);
            if (!exists) return null;

            var logs = await _db.WeightLogs.AsNoTracking()
                .Where(w => w.MemberId == memberId)
                .OrderByDescending(w => w.LogDate)
                .ThenByDescending(w => w.Id)
                .ToListAsync(ct);

            return logs;
        }

        public async Task AddWeightAsync(WeightLog log, CancellationToken ct)
        {
            _db.WeightLogs.Add(log);
            await _db.SaveChangesAsync(ct);
        }

        public Task<WeightLog?> GetWeightByIdAsync(long memberId, long logId, CancellationToken ct) =>
            _db.WeightLogs.FirstOrDefaultAsync(w => w.MemberId == memberId && w.Id == logId, ct);

        public async Task DeleteWeightAsync(WeightLog log, CancellationToken ct)
        {
            _db.WeightLogs.Remove(log);
            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> MemberExistsAsync(long memberId, CancellationToken ct) =>
            _db.Members.AnyAsync(m => m.Id == memberId, ct);

        public Task<bool> MemberCodeExistsAsync(string code, CancellationToken ct) =>
            _db.Members.AnyAsync(m => m.MemberCode == code, ct);
    }
}

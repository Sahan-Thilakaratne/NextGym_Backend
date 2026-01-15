using Contracts.Members;
using Domain.Members;
using Infrastructure.Members;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Application.Members
{
    public sealed class MemberService : IMemberService
    {

        private readonly IMemberRepository _repo;

        public MemberService(IMemberRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResponse<MemberListItemDto>> GetMembersAsync(GetMembersQuery q, CancellationToken ct)
        {
            var (items, total) = await _repo.GetPagedAsync(q.Search, q.Page, q.PageSize, ct);
            var dtos = items.Select(m => new MemberListItemDto(
                m.Id, m.MemberCode, m.FirstName, m.LastName, m.Mobile, m.Status.ToString(), m.JoinDate
            )).ToList();

            return new PagedResponse<MemberListItemDto>(dtos, q.Page, q.PageSize, total);
        }

        public async Task<MemberDetailDto?> GetMemberByIdAsync(long id, CancellationToken ct)
        {
            var member = await _repo.GetByIdWithHealthAsync(id, ct);
            if (member is null) return null;

            var hp = member.HealthProfile is null ? null : new HealthProfileDto(
                member.HealthProfile.Id,
                member.HealthProfile.MemberId,
                member.HealthProfile.HeightCm,
                member.HealthProfile.RestingHr,
                member.HealthProfile.BloodPressure,
                SafeJson(member.HealthProfile.ConditionsJson),
                member.HealthProfile.Notes
            );

            var bmi = await GetLatestBmiAsync(id, ct);

            return new MemberDetailDto(
                member.Id,
                member.MemberCode,
                member.FirstName,
                member.LastName,
                member.Mobile,
                member.Email,
                member.Dob,
                member.Gender,
                member.Address,
                member.EmergencyContactName,
                member.EmergencyContactPhone,
                member.JoinDate,
                member.Status.ToString(),
                hp,
                bmi
            );
        }

        public async Task<MemberDetailDto> CreateMemberAsync(CreateMemberRequest req, CancellationToken ct)
        {
            var status = ParseStatus(req.Status);
            var joinDate = req.JoinDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var code = string.IsNullOrWhiteSpace(req.MemberCode)
                ? await GenerateMemberCodeAsync(ct)
                : req.MemberCode.Trim();

            if (await _repo.MemberCodeExistsAsync(code, ct))
                throw new InvalidOperationException("Member code already exists.");

            var member = new Member
            {
                MemberCode = code,
                FirstName = req.FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(req.LastName) ? null : req.LastName.Trim(),
                Mobile = req.Mobile.Trim(),
                Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim(),
                Dob = req.Dob,
                Gender = string.IsNullOrWhiteSpace(req.Gender) ? null : req.Gender.Trim(),
                Address = string.IsNullOrWhiteSpace(req.Address) ? null : req.Address.Trim(),
                EmergencyContactName = string.IsNullOrWhiteSpace(req.EmergencyContactName) ? null : req.EmergencyContactName.Trim(),
                EmergencyContactPhone = string.IsNullOrWhiteSpace(req.EmergencyContactPhone) ? null : req.EmergencyContactPhone.Trim(),
                JoinDate = joinDate,
                Status = status
            };

            await _repo.AddAsync(member, ct);

            // Return detail
            return (await GetMemberByIdAsync(member.Id, ct))!;
        }

        public async Task<MemberDetailDto?> UpdateMemberAsync(long id, UpdateMemberRequest req, CancellationToken ct)
        {
            // Load tracked entity for update:
            // simplest: get by id then construct tracked member by querying without AsNoTracking in repo.
            // We'll just do a re-fetch using GetByIdAsync and then update via repo.UpdateAsync with a new entity.

            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing is null) return null;

            var status = ParseStatus(req.Status);

            existing.FirstName = req.FirstName.Trim();
            existing.LastName = string.IsNullOrWhiteSpace(req.LastName) ? null : req.LastName.Trim();
            existing.Mobile = req.Mobile.Trim();
            existing.Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim();
            existing.Dob = req.Dob;
            existing.Gender = string.IsNullOrWhiteSpace(req.Gender) ? null : req.Gender.Trim();
            existing.Address = string.IsNullOrWhiteSpace(req.Address) ? null : req.Address.Trim();
            existing.EmergencyContactName = string.IsNullOrWhiteSpace(req.EmergencyContactName) ? null : req.EmergencyContactName.Trim();
            existing.EmergencyContactPhone = string.IsNullOrWhiteSpace(req.EmergencyContactPhone) ? null : req.EmergencyContactPhone.Trim();
            existing.JoinDate = req.JoinDate ?? existing.JoinDate;
            existing.Status = status;

            await _repo.UpdateAsync(existing, ct);

            return await GetMemberByIdAsync(id, ct);
        }

        public async Task<bool> DeleteMemberAsync(long id, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing is null) return false;

            // If member has subscriptions, MySQL FK RESTRICT will fail.
            // Let it throw; your API will return 500 unless you add global exception handling later.
            await _repo.DeleteAsync(existing, ct);
            return true;
        }

        public async Task<HealthProfileDto?> GetHealthProfileAsync(long memberId, CancellationToken ct)
        {
            var hp = await _repo.GetHealthAsync(memberId, ct);
            if (hp is null) return null;

            return new HealthProfileDto(hp.Id, hp.MemberId, hp.HeightCm, hp.RestingHr, hp.BloodPressure, SafeJson(hp.ConditionsJson), hp.Notes);
        }

        public async Task<HealthProfileDto?> UpsertHealthProfileAsync(long memberId, UpsertHealthProfileRequest req, CancellationToken ct)
        {
            if (!await _repo.MemberExistsAsync(memberId, ct)) return null;

            var json = NormalizeJson(req.ConditionsJson);

            var profile = new HealthProfile
            {
                MemberId = memberId,
                HeightCm = req.HeightCm,
                RestingHr = req.RestingHr,
                BloodPressure = string.IsNullOrWhiteSpace(req.BloodPressure) ? null : req.BloodPressure.Trim(),
                ConditionsJson = json,
                Notes = string.IsNullOrWhiteSpace(req.Notes) ? null : req.Notes.Trim(),
            };

            await _repo.UpsertHealthAsync(profile, ct);
            return await GetHealthProfileAsync(memberId, ct);
        }

        public async Task<List<WeightLogDto>?> GetWeightLogsAsync(long memberId, CancellationToken ct)
        {
            var logs = await _repo.GetWeightsAsync(memberId, ct);
            if (logs is null) return null;

            return logs.Select(w => new WeightLogDto(w.Id, w.MemberId, w.LogDate, w.WeightKg)).ToList();
        }

        public async Task<WeightLogDto?> AddWeightLogAsync(long memberId, AddWeightLogRequest req, CancellationToken ct)
        {
            if (!await _repo.MemberExistsAsync(memberId, ct)) return null;

            var log = new WeightLog
            {
                MemberId = memberId,
                LogDate = req.LogDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                WeightKg = req.WeightKg
            };

            await _repo.AddWeightAsync(log, ct);
            return new WeightLogDto(log.Id, log.MemberId, log.LogDate, log.WeightKg);
        }

        public async Task<bool> DeleteWeightLogAsync(long memberId, long logId, CancellationToken ct)
        {
            var log = await _repo.GetWeightByIdAsync(memberId, logId, ct);
            if (log is null) return false;

            await _repo.DeleteWeightAsync(log, ct);
            return true;
        }

        public async Task<BmiLatestDto?> GetLatestBmiAsync(long memberId, CancellationToken ct)
        {
            var hp = await _repo.GetHealthAsync(memberId, ct);
            if (hp?.HeightCm is null || hp.HeightCm <= 0) return null;

            var weights = await _repo.GetWeightsAsync(memberId, ct);
            var latest = weights?.FirstOrDefault();
            if (latest is null) return null;

            var heightM = hp.HeightCm.Value / 100m;
            var bmi = Math.Round(latest.WeightKg / (heightM * heightM), 2);

            var category = bmi switch
            {
                < 18.5m => "Underweight",
                < 25.0m => "Normal",
                < 30.0m => "Overweight",
                _ => "Obese"
            };

            return new BmiLatestDto(memberId, latest.LogDate, latest.WeightKg, hp.HeightCm.Value, bmi, category);
        }

        private static MemberStatus ParseStatus(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return MemberStatus.ACTIVE;
            return Enum.TryParse<MemberStatus>(s.Trim(), true, out var st) ? st : MemberStatus.ACTIVE;
        }

        private static string NormalizeJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return "{}";
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetRawText();
            }
            catch
            {
                // If invalid JSON comes in, fail hard (better than storing garbage)
                throw new InvalidOperationException("conditions_json must be valid JSON.");
            }
        }

        private static string SafeJson(string? json) => string.IsNullOrWhiteSpace(json) ? "{}" : json;

        private async Task<string> GenerateMemberCodeAsync(CancellationToken ct)
        {
            // Format: M-YYYY-XXXXX
            // Example: M-2026-48312
            var year = DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture);
            for (var i = 0; i < 10; i++)
            {
                var rand = Random.Shared.Next(10000, 99999).ToString(CultureInfo.InvariantCulture);
                var code = $"M-{year}-{rand}";
                if (!await _repo.MemberCodeExistsAsync(code, ct))
                    return code;
            }

            // extremely unlikely fallback
            return $"M-{year}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Members
{
    public sealed record HealthProfileDto(
    long Id,
    long MemberId,
    decimal? HeightCm,
    int? RestingHr,
    string? BloodPressure,
    string ConditionsJson,
    string? Notes);

    public sealed record WeightLogDto(
        long Id,
        long MemberId,
        DateOnly LogDate,
        decimal WeightKg);

    public sealed record BmiLatestDto(
        long MemberId,
        DateOnly LogDate,
        decimal WeightKg,
        decimal HeightCm,
        decimal Bmi,
        string Category);
}

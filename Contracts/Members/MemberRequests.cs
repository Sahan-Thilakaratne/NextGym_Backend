using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Members
{
    public sealed record CreateMemberRequest(
    string? MemberCode,
    string FirstName,
    string? LastName,
    string Mobile,
    string? Email,
    DateOnly? Dob,
    string? Gender,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    DateOnly? JoinDate,
    string? Status // ACTIVE/INACTIVE/SUSPENDED
);

    public sealed record UpdateMemberRequest(
        string FirstName,
        string? LastName,
        string Mobile,
        string? Email,
        DateOnly? Dob,
        string? Gender,
        string? Address,
        string? EmergencyContactName,
        string? EmergencyContactPhone,
        DateOnly? JoinDate,
        string? Status
    );

    public sealed record UpsertHealthProfileRequest(
        decimal? HeightCm,
        int? RestingHr,
        string? BloodPressure,
        string? ConditionsJson, // pass raw JSON string OR null
        string? Notes
    );

    public sealed record AddWeightLogRequest(
        DateOnly? LogDate,
        decimal WeightKg
    );
}

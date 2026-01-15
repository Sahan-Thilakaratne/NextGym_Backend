using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Members
{
    public sealed record MemberListItemDto(
    long Id,
    string MemberCode,
    string FirstName,
    string? LastName,
    string Mobile,
    string Status,
    DateOnly JoinDate);

    public sealed record MemberDetailDto(
        long Id,
        string MemberCode,
        string FirstName,
        string? LastName,
        string Mobile,
        string? Email,
        DateOnly? Dob,
        string? Gender,
        string? Address,
        string? EmergencyContactName,
        string? EmergencyContactPhone,
        DateOnly JoinDate,
        string Status,
        HealthProfileDto? HealthProfile,
        BmiLatestDto? LatestBmi);
}

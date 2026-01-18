using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Billing
{
    public sealed record SubscriptionListItemDto(
    long Id,
    long MemberId,
    long PackageId,
    string PackageName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    decimal TotalPayable,
    decimal TotalPaid,
    decimal Outstanding,
    int DaysToExpiry
);

    public sealed record SubscriptionDetailDto(
        long Id,
        long MemberId,
        long PackageId,
        string PackageName,
        int PackageDurationDays,
        DateOnly StartDate,
        DateOnly EndDate,
        string Status,
        decimal Amount,
        decimal Discount,
        decimal Taxes,
        decimal TotalPayable,
        decimal TotalPaid,
        decimal Outstanding,
        string? Notes,
        IReadOnlyList<PaymentDto> Payments
    );

    public sealed record CreateSubscriptionRequest(
        long MemberId,
        long PackageId,
        DateOnly? StartDate,
        DateOnly? EndDate,      // optional (DB trigger can compute)
        decimal? Amount,        // optional (DB trigger can default from package)
        decimal? Discount,
        decimal? Taxes,
        string? Notes
    );

    // For pause / resume / expire (admin actions)
    public sealed record UpdateSubscriptionStatusRequest(
        string Status // ACTIVE / PAUSED / EXPIRED
    );

    public sealed record MemberDuesDto(
        long MemberId,
        decimal TotalPayable,
        decimal TotalPaid,
        decimal Outstanding
    );
}

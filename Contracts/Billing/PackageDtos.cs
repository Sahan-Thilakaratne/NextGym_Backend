using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Billing
{
    public sealed record PackageDto(
    long Id,
    string Name,
    string? Description,
    int DurationDays,
    decimal Price,
    int? SessionLimit,
    bool IsActive
);

    public sealed record CreatePackageRequest(
        string Name,
        string? Description,
        int DurationDays,
        decimal Price,
        int? SessionLimit,
        bool? IsActive
    );

    public sealed record UpdatePackageRequest(
        string Name,
        string? Description,
        int DurationDays,
        decimal Price,
        int? SessionLimit,
        bool? IsActive
    );
}

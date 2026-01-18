using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Billing
{
    public sealed record PaymentDto(
    long Id,
    long SubscriptionId,
    DateTime PaidAt,
    string Method,
    decimal Amount,
    string? Reference,
    string Status
);

    public sealed record RecordPaymentRequest(
        long SubscriptionId,
        string Method,          // CASH/CARD/ONLINE
        decimal Amount,
        string? Reference,
        DateTime? PaidAt,
        string? Status          // PAID/FAILED/REFUNDED (optional)
    );

    public sealed record ReceiptDto(
        long PaymentId,
        DateTime PaidAt,
        decimal Amount,
        string Method,
        string? Reference,
        long SubscriptionId,
        long MemberId,
        string MemberCode,
        string MemberName,
        string PackageName,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal TotalPayable,
        decimal TotalPaid,
        decimal Outstanding
    );
}

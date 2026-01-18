using Contracts.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing
{
    public interface IPaymentService
    {
        Task<PaymentDto> RecordAsync(RecordPaymentRequest req, CancellationToken ct);
        Task<IReadOnlyList<PaymentDto>> GetBySubscriptionAsync(long subscriptionId, CancellationToken ct);
        Task<ReceiptDto?> GetReceiptAsync(long paymentId, CancellationToken ct);
    }
}

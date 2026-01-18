using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Billing
{
    public class Payment
    {
        public long Id { get; set; }

        public long SubscriptionId { get; set; }
        public DateTime PaidAt { get; set; }

        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.PAID;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Subscription Subscription { get; set; } = null!;
    }
}

using Domain.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Billing
{
    public class Subscription
    {
        public long Id { get; set; }

        public long MemberId { get; set; }
        public long PackageId { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.ACTIVE;

        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalPayable { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Member Member { get; set; } = null!;
        public Package Package { get; set; } = null!;
        public List<Payment> Payments { get; set; } = new();
    }
}

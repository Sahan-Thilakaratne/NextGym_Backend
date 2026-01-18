using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Billing
{
    public class Package
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public int? SessionLimit { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Subscription> Subscriptions { get; set; } = new();
    }
}

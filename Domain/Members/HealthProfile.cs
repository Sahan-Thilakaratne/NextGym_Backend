using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Members
{
    public class HealthProfile
    {

        public long Id { get; set; }
        public long MemberId { get; set; }

        public decimal? HeightCm { get; set; }
        public int? RestingHr { get; set; }
        public string? BloodPressure { get; set; }

        // stored as JSON in DB
        public string ConditionsJson { get; set; } = "{}";
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Member Member { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Members
{
    public class WeightLog
    {

        public long Id { get; set; }
        public long MemberId { get; set; }

        public DateOnly LogDate { get; set; }
        public decimal WeightKg { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Member Member { get; set; } = null!;
    }
}

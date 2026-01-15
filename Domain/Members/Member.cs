using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Members
{
    public class Member
    {

        public long Id { get; set; }

        public string MemberCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateOnly JoinDate { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.ACTIVE;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public HealthProfile? HealthProfile { get; set; }
        public List<WeightLog> WeightLogs { get; set; } = new();

    }
}

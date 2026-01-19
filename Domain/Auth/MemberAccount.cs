using Domain.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Auth
{
    public class MemberAccount
    {
        public long Id { get; set; }

        public long MemberId { get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }

        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Member Member { get; set; } = null!;
    }
}

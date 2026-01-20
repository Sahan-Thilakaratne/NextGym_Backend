using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Users
{
    public class User
    {
        public long Id { get; set; }
        public UserRole Role { get; set; }

        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

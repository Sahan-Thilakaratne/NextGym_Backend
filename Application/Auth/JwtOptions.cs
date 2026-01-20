using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = "NextGym";
        public string Audience { get; set; } = "NextGym";
        public string Key { get; set; } = "";
        public int ExpiresMinutes { get; set; } = 720;
    }
}

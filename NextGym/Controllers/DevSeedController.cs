using Domain.Users;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NextGym.Api.Controllers
{
    [ApiController]
    [Route("api/dev/seed")]
    public class DevSeedController : ControllerBase
    {
        private readonly IUserRepository _users;
        public DevSeedController(IUserRepository users) => _users = users;

        [HttpPost("admin")]
        public async Task<IActionResult> SeedAdmin(CancellationToken ct)
        {
            var existing = await _users.GetByUsernameAsync("admin", ct);
            if (existing is not null) return Ok("admin already exists");

            var u = new User
            {
                Username = "admin",
                Name = "System Admin",
                Role = UserRole.ADMIN,
                IsActive = true
            };

            var hasher = new PasswordHasher<User>();
            u.PasswordHash = hasher.HashPassword(u, "Admin@123");

            await _users.AddAsync(u, ct);
            return Ok("admin created (admin / Admin@123)");
        }
    }
}

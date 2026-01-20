using Application.Auth;
using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NextGym.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var res = await _auth.LoginAsync(req, ct);
            return Ok(res);
        }

        // ADMIN creates other staff users
        [Authorize(Roles = "ADMIN")]
        [HttpPost("register")]
        public async Task<ActionResult<MeResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
        {
            var created = await _auth.RegisterAsync(req, ct);
            return Ok(created);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<MeResponse>> Me(CancellationToken ct)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(idStr, out var userId)) return Unauthorized();

            var me = await _auth.GetMeAsync(userId, ct);
            return Ok(me);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(idStr, out var userId)) return Unauthorized();

            await _auth.ChangePasswordAsync(userId, req, ct);
            return NoContent();
        }
    }
}

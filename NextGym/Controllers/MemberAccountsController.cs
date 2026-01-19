using Application.Auth;
using Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace NextGym.Api.Controllers
{
    [ApiController]
    [Route("api/members/{memberId:long}/account")]
    public class MemberAccountsController : ControllerBase
    {
        private readonly IMemberAccountService _service;
        public MemberAccountsController(IMemberAccountService service) => _service = service;

        // GET /api/members/{memberId}/account
        [HttpGet]
        public async Task<ActionResult<MemberAccountDto>> Get(long memberId, CancellationToken ct)
        {
            var acc = await _service.GetByMemberIdAsync(memberId, ct);
            return acc is null ? NotFound() : Ok(acc);
        }

        // POST /api/members/{memberId}/account
        [HttpPost]
        public async Task<ActionResult<MemberAccountDto>> Create(long memberId, [FromBody] CreateMemberAccountRequest req, CancellationToken ct)
        {
            if (req.MemberId != memberId)
                req = req with { MemberId = memberId };

            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(Get), new { memberId }, created);
        }

        // PUT /api/members/{memberId}/account
        [HttpPut]
        public async Task<ActionResult<MemberAccountDto>> Update(long memberId, [FromBody] UpdateMemberAccountRequest req, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(memberId, req, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE /api/members/{memberId}/account
        [HttpDelete]
        public async Task<IActionResult> Delete(long memberId, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(memberId, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}

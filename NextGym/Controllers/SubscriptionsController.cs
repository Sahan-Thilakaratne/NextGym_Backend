using Application.Billing;
using Contracts.Billing;
using Contracts.Members;
using Microsoft.AspNetCore.Mvc;

namespace NextGym.Api.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _service;
    public SubscriptionsController(ISubscriptionService service) => _service = service;

    // GET /api/subscriptions?memberId=1&status=ACTIVE&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<PagedResponse<SubscriptionListItemDto>>> GetAll(
        [FromQuery] long? memberId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(memberId, status, page, pageSize, ct);
        return Ok(result);
    }

    // GET /api/subscriptions/{id}
    [HttpGet("{id:long}")]
    public async Task<ActionResult<SubscriptionDetailDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    // POST /api/subscriptions (new/renew)
    [HttpPost]
    public async Task<ActionResult<SubscriptionDetailDto>> Create([FromBody] CreateSubscriptionRequest req, CancellationToken ct)
    {
        var created = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PATCH /api/subscriptions/{id}/status  (pause/resume/expire)
    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<SubscriptionDetailDto>> UpdateStatus(long id, [FromBody] UpdateSubscriptionStatusRequest req, CancellationToken ct)
    {
        var updated = await _service.UpdateStatusAsync(id, req, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    // GET /api/subscriptions/member/{memberId}/dues
    [HttpGet("member/{memberId:long}/dues")]
    public async Task<ActionResult<MemberDuesDto>> GetMemberDues(long memberId, CancellationToken ct)
    {
        var dues = await _service.GetMemberDuesAsync(memberId, ct);
        return dues is null ? NotFound() : Ok(dues);
    }
}

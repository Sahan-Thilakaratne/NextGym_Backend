using Application.Members;
using Contracts.Members;
using Microsoft.AspNetCore.Mvc;

namespace NextGym.Api.Controllers
{

    [ApiController]
    [Route("api/members")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        // GET /api/members?search=dinusha&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResponse<MemberListItemDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _service.GetMembersAsync(new GetMembersQuery(search, page, pageSize), ct);
            return Ok(result);
        }

        // GET /api/members/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<MemberDetailDto>> GetById(long id, CancellationToken ct)
        {
            var member = await _service.GetMemberByIdAsync(id, ct);
            return member is null ? NotFound() : Ok(member);
        }

        // POST /api/members
        [HttpPost]
        public async Task<ActionResult<MemberDetailDto>> Create([FromBody] CreateMemberRequest req, CancellationToken ct)
        {
            var created = await _service.CreateMemberAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /api/members/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<MemberDetailDto>> Update(long id, [FromBody] UpdateMemberRequest req, CancellationToken ct)
        {
            var updated = await _service.UpdateMemberAsync(id, req, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE /api/members/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            var ok = await _service.DeleteMemberAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }

        // GET /api/members/{id}/health
        [HttpGet("{id:long}/health")]
        public async Task<ActionResult<HealthProfileDto>> GetHealth(long id, CancellationToken ct)
        {
            var hp = await _service.GetHealthProfileAsync(id, ct);
            return hp is null ? NotFound() : Ok(hp);
        }

        // PUT /api/members/{id}/health
        [HttpPut("{id:long}/health")]
        public async Task<ActionResult<HealthProfileDto>> UpsertHealth(long id, [FromBody] UpsertHealthProfileRequest req, CancellationToken ct)
        {
            var hp = await _service.UpsertHealthProfileAsync(id, req, ct);
            return hp is null ? NotFound() : Ok(hp);
        }

        // GET /api/members/{id}/weights
        [HttpGet("{id:long}/weights")]
        public async Task<ActionResult<List<WeightLogDto>>> GetWeights(long id, CancellationToken ct)
        {
            var weights = await _service.GetWeightLogsAsync(id, ct);
            return weights is null ? NotFound() : Ok(weights);
        }

        // POST /api/members/{id}/weights
        [HttpPost("{id:long}/weights")]
        public async Task<ActionResult<WeightLogDto>> AddWeight(long id, [FromBody] AddWeightLogRequest req, CancellationToken ct)
        {
            var created = await _service.AddWeightLogAsync(id, req, ct);
            return created is null ? NotFound() : Ok(created);
        }

        // DELETE /api/members/{id}/weights/{logId}
        [HttpDelete("{id:long}/weights/{logId:long}")]
        public async Task<IActionResult> DeleteWeight(long id, long logId, CancellationToken ct)
        {
            var ok = await _service.DeleteWeightLogAsync(id, logId, ct);
            return ok ? NoContent() : NotFound();
        }

        // GET /api/members/{id}/bmi/latest
        [HttpGet("{id:long}/bmi/latest")]
        public async Task<ActionResult<BmiLatestDto>> GetLatestBmi(long id, CancellationToken ct)
        {
            var bmi = await _service.GetLatestBmiAsync(id, ct);
            return bmi is null ? NotFound() : Ok(bmi);
        }
    }
}

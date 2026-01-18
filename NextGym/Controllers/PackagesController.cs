using Application.Billing;
using Contracts.Billing;
using Microsoft.AspNetCore.Mvc;


namespace NextGym.Api.Controllers;

[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _service;
    public PackagesController(IPackageService service) => _service = service;

    // GET /api/packages?onlyActive=true
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PackageDto>>> GetAll([FromQuery] bool? onlyActive, CancellationToken ct)
        => Ok(await _service.GetAllAsync(onlyActive, ct));

    // GET /api/packages/{id}
    [HttpGet("{id:long}")]
    public async Task<ActionResult<PackageDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    // POST /api/packages
    [HttpPost]
    public async Task<ActionResult<PackageDto>> Create([FromBody] CreatePackageRequest req, CancellationToken ct)
    {
        var created = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/packages/{id}
    [HttpPut("{id:long}")]
    public async Task<ActionResult<PackageDto>> Update(long id, [FromBody] UpdatePackageRequest req, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, req, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/packages/{id}
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}

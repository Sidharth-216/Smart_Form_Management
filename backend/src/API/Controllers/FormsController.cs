using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace API.Controllers;

[ApiController]
[Route("api/forms")]
public sealed class FormsController(FormService formService, UploadService uploadService) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<FormDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 12, CancellationToken cancellationToken = default)
        => formService.GetFormsAsync(new PaginationQuery(page, pageSize), cancellationToken);

    [HttpGet("facets")]
    public Task<FormBrowseFacetsDto> GetFacets(CancellationToken cancellationToken = default)
        => formService.GetBrowseFacetsAsync(cancellationToken);

    [HttpGet("{id}")]
    public async Task<ActionResult<FormDto>> GetById(string id, CancellationToken cancellationToken = default)
    {
        var form = await formService.GetByIdAsync(id, cancellationToken);
        return form is null ? NotFound() : Ok(form);
    }

    [HttpGet("search")]
    public Task<PagedResult<FormDto>> Search([FromQuery] string q = "", [FromQuery] string? country = null, [FromQuery] string? state = null, [FromQuery] string? department = null, [FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 12, CancellationToken cancellationToken = default)
        => formService.SearchAsync(q, country, state, department, category, new PaginationQuery(page, pageSize), cancellationToken);

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FormDto>> Create([FromBody] FormUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var uploadedBy = User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "admin";
        var created = await formService.CreateAsync(request, uploadedBy, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Update(string id, [FromBody] FormUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var updated = await formService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var deleted = await formService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("upload")]
    [Authorize(Roles = "admin")]
    [RequestSizeLimit(25_000_000)]
    public async Task<ActionResult<FormDto>> Upload([FromForm] UploadFormRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "admin";
        var created = await uploadService.UploadAsync(userId, request, cancellationToken);
        return Ok(created);
    }
}

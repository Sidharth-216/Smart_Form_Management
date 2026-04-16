using Application.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/admin/uploads")]
[Authorize(Roles = "admin")]
public sealed class AdminController(IUploadRepository uploads) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken = default)
        => Ok(await uploads.GetPendingAsync(cancellationToken));

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(string id, CancellationToken cancellationToken = default)
        => Ok(new { updated = await uploads.UpdateStatusAsync(id, UploadStatus.Approved, cancellationToken) });

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(string id, CancellationToken cancellationToken = default)
        => Ok(new { updated = await uploads.UpdateStatusAsync(id, UploadStatus.Rejected, cancellationToken) });
}

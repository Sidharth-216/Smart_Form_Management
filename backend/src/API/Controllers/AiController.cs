using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public sealed class AiController(IAiClient aiClient) : ControllerBase
{
    [HttpPost("extract-text")]
    public async Task<IActionResult> ExtractText([FromBody] AiExtractTextRequest request, CancellationToken cancellationToken = default)
        => Ok(new { text = await aiClient.ExtractTextAsync(request.FileUrl, cancellationToken) });

    [HttpPost("classify")]
    public async Task<IActionResult> Classify([FromBody] AiClassifyRequest request, CancellationToken cancellationToken = default)
        => Ok(await aiClient.ClassifyAsync(request.Text, cancellationToken));

    [HttpPost("suggest-tags")]
    public async Task<IActionResult> SuggestTags([FromBody] AiSuggestTagsRequest request, CancellationToken cancellationToken = default)
        => Ok(new { tags = await aiClient.SuggestTagsAsync(request.Text, cancellationToken) });
}

using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed record AiExtractTextRequest([property: Required, MinLength(3), MaxLength(2048)] string FileUrl);

public sealed record AiClassifyRequest([property: Required, MinLength(1), MaxLength(10000)] string Text);

public sealed record AiSuggestTagsRequest([property: Required, MinLength(1), MaxLength(10000)] string Text);
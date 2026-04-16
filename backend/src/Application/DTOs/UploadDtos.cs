using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public sealed record UploadFormRequest(
    [property: Required, MinLength(2), MaxLength(180)] string Title,
    [property: Required, MaxLength(2000)] string Description,
    [property: Required, MinLength(2), MaxLength(120)] string Country,
    [property: Required, MinLength(2), MaxLength(120)] string State,
    [property: Required, MinLength(2), MaxLength(160)] string Department,
    [property: Required, MinLength(2), MaxLength(160)] string Category,
    [property: MaxLength(64)] string Version,
    [property: MaxLength(300)] string LanguageCsv,
    [property: MaxLength(600)] string KeywordsCsv,
    [property: Required] IFormFile File);

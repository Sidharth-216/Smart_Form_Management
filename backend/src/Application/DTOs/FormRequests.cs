using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed record FormUpsertRequest(
    [property: Required, MinLength(2), MaxLength(180)] string Title,
    [property: Required, MaxLength(2000)] string Description,
    [property: Required, MinLength(2), MaxLength(120)] string Country,
    [property: Required, MinLength(2), MaxLength(120)] string State,
    [property: Required, MinLength(2), MaxLength(160)] string Department,
    [property: Required, MinLength(2), MaxLength(160)] string Category,
    [property: MaxLength(64)] string Version,
    [property: MaxLength(600)] string KeywordsCsv = "",
    [property: MaxLength(300)] string LanguageCsv = "English");

public sealed record FormBrowseFacetsDto(
    IReadOnlyList<string> Countries,
    IReadOnlyList<string> States,
    IReadOnlyList<string> Departments,
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> Languages);
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed record RegisterRequest(
	[property: Required, MinLength(2), MaxLength(120)] string Name,
	[property: Required, EmailAddress, MaxLength(254)] string Email,
	[property: Required, MinLength(8), MaxLength(128)] string Password,
	string Role = "user");

public sealed record LoginRequest(
	[property: Required, EmailAddress, MaxLength(254)] string Email,
	[property: Required, MinLength(8), MaxLength(128)] string Password);

public sealed record AuthResponse(string Token, string UserId, string Name, string Email, string Role);

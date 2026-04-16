using Application.Contracts;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services;

public sealed class AuthService(IUserRepository users, IJwtTokenService tokens)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await users.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        };

        var created = await users.CreateAsync(user, cancellationToken);
        var token = tokens.CreateToken(created);
        return new AuthResponse(token, created.Id, created.Name, created.Email, created.Role.ToString().ToLowerInvariant());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken)
            ?? throw new InvalidOperationException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var token = tokens.CreateToken(user);
        return new AuthResponse(token, user.Id, user.Name, user.Email, user.Role.ToString().ToLowerInvariant());
    }
}

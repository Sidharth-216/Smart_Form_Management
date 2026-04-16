using Domain.Entities;

namespace Application.Contracts;

public interface IJwtTokenService
{
    string CreateToken(UserAccount user);
}

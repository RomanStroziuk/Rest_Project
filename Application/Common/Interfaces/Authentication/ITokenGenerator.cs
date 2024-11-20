using Domain.Users;


namespace Application.Common.Interfaces.Authentication;

public interface ITokenGenerator
{
    string GenerateToken(User user);
    
}
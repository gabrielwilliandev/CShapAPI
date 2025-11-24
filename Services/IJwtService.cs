using API.Models;

namespace API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

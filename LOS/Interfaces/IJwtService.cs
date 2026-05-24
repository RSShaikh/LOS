using LOS.Models;

namespace LOS.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Customer customer);
    }
}

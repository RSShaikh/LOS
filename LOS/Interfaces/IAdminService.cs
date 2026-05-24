using LOS.Models;

namespace LOS.Interfaces
{
    public interface IAdminService
    {
        List<Customer> GetAllCustomersWithKyc();
    }
}

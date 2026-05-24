using LOS.Models;

namespace LOS.Interfaces
{
    public interface ICibilService
    {
        Task<Customer> GetCustomerWithCibilAsync(int customerId);
        Task<int>
  GenerateAndSaveCibilScoreAsync(
  int customerId,
  decimal bankBalance
  );

        Task<List<Customer>> GetAllCustomersAsync();
    }
}
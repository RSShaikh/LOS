using LOS.Models;

namespace LOS.Interfaces
{
    public interface ICustomerService
    {
        Customer GetById(int customerId);
        Customer GetByEmail(string email);
        bool EmailExists(string email);
        bool PanExists(string pan);
        bool AadhaarExists(string aadhaarNo);
        void Add(Customer customer);
        void Update(Customer customer);
        void SaveOtp(string email, string otp);
        bool VerifyOtp(string email, string otp);
    }
}

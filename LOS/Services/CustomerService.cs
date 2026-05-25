using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class CustomerService : ICustomerService
    {
        ApplicationDbContext db;

        public CustomerService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public Customer GetById(int customerId)
        {
            return db.Customers
                .Include(c => c.KYCDocument)
                .FirstOrDefault(c => c.CustomerId == customerId);
        }

        public Customer GetByEmail(string email)
        {
            return db.Customers
                .FirstOrDefault(c => c.Email == email);
        }

        public bool EmailExists(string email)
        {
            return db.Customers.Any(c => c.Email == email);
        }

        public bool PanExists(string pan)
        {
            return db.Customers.Any(c => c.PAN == pan);
        }

        public bool AadhaarExists(string aadhaarNo)
        {
            return db.Customers.Any(c => c.AadhaarNo == aadhaarNo);
        }

        public void Add(Customer customer)
        {
            db.Customers.Add(customer);
            db.SaveChanges();
        }

        public void Update(Customer customer)
        {
            db.Customers.Update(customer);
            db.SaveChanges();
        }

        public void SaveOtp(string email, string otp)
        {
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);

            if (customer != null)
            {
                customer.OtpCode = otp;
                customer.OtpExpiry = DateTime.Now.AddMinutes(10);
                db.SaveChanges();
            }
        }

        public bool VerifyOtp(string email, string otp)
        {
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);

            if (customer == null) return false;
            if (customer.OtpCode != otp) return false;
            if (customer.OtpExpiry < DateTime.Now) return false;

            customer.OtpCode = null;
            customer.OtpExpiry = null;
            db.SaveChanges();

            return true;
        }

        public Customer GetByIdWithCibil(int customerId)
        {
            return db.Customers
                .Include(c => c.CibilReport)
                .FirstOrDefault(c => c.CustomerId == customerId);
        }
    }
}
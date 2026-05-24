using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class AdminService : IAdminService
    {
        ApplicationDbContext db;

        public AdminService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public List<Customer> GetAllCustomersWithKyc()
        {
            return db.Customers
                .Include(c => c.KYCDocument)
                .Where(c => c.Role == "Customer")
                .OrderByDescending(c => c.CustomerId)
                .ToList();
        }
    }
}

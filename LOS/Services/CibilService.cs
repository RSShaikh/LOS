using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class CibilService : ICibilService
    {
        private readonly ApplicationDbContext _context;

        public CibilService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer>
        GetCustomerWithCibilAsync(
            int customerId)
        {
            return await _context.Customers
                .Include(
                    c => c.CibilReport
                )
                .Include(
                    c => c.KYCDocument
                )
                .FirstOrDefaultAsync(
                    c =>
                    c.CustomerId
                    ==
                    customerId
                );
        }

        public async Task<int>
       GenerateAndSaveCibilScoreAsync(
int customerId,
decimal bankBalance
)
        {
            var customer =
                await _context.Customers

                .Include(
                    c =>
                    c.CibilReport
                )

                .Include(
                    c =>
                    c.KYCDocument
                )

                .FirstOrDefaultAsync(
                    c =>
                    c.CustomerId
                    ==
                    customerId
                );

            if (
                customer
                ==
                null
            )
            {
                throw new Exception(
                    "Customer not found."
                );
            }

            // KYC APPROVAL CHECK

            bool kycApproved =

                customer
                .KYCDocument

                !=

                null

                &&

                customer
                .KYCDocument
                .Any(

                x =>

                x.IsActive

                &&

                x.VerificationStatus
                ==
                "Approved"

                );

            if (
                !kycApproved
            )
            {
                throw new Exception(

                "Complete KYC and wait for officer approval."

                );
            }

            int score = 500;

            // SALARY

            if (
                customer.MonthlyIncome
                >
                100000
            )
            {
                score += 300;
            }

            else if (
                customer.MonthlyIncome
                >
                50000
            )
            {
                score += 200;
            }

            // EMPLOYMENT

            if (
                customer
                .EmploymentType
                ?
                .Trim()
                .ToLower()

                ==

                "salaried"
            )
            {
                score += 100;
            }

            // AGE

            if (
customer.Age >= 25
&&
customer.Age <= 45
)
            {
                score += 100;
            }

            // BANK BALANCE

            if (
                bankBalance
                >
                500000
            )
            {
                score += 150;
            }

            // MAX SCORE

            score =
                Math.Min(
                    score,
                    900
                );

            var latestReport =

                customer
                .CibilReport
                ?
                .OrderByDescending(
                    r =>
                    r.CheckDate
                )
                .FirstOrDefault();

            if (
                latestReport
                !=
                null
            )
            {
                latestReport
                .CibilScore
                =
                score;

                latestReport
                .CheckDate
                =
                DateTime.Now;
            }
            else
            {
                customer
                .CibilReport
                ??=

                new List<CibilReport>();

                customer
                .CibilReport
                .Add(

                new CibilReport
                {
                    CustomerId =
                    customer.CustomerId,

                    PAN =
                    customer.PAN
                    ??
                    "",

                    CibilScore =
                    score,

                    CheckDate =
                    DateTime.Now
                }

                );
            }

            await _context
            .SaveChangesAsync();

            return score;
        }

        public async Task<List<Customer>>
        GetAllCustomersAsync()
        {
            return await _context.Customers

                .Include(
                    x =>
                    x.CibilReport
                )

                .ToListAsync();
        }
    }
}
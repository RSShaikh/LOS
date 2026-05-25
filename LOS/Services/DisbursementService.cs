using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class DisbursementService : IDisbursementService
    {
        private readonly ApplicationDbContext db;
        private readonly IEmailService emailService;

        public DisbursementService(ApplicationDbContext db, IEmailService emailService)
        {
            this.db = db;
            this.emailService = emailService;
        }

        public void CreateDisbursement(Disbursement disbursement)
        {
            disbursement.DisbursementDate = DateTime.Now;
            disbursement.DisbursementStatus = "Completed";

            db.Disbursements.Add(disbursement);

            // Update sanction letter status
            var sanction = db.SanctionLetters.FirstOrDefault(s => s.LoanDealId == disbursement.LoanDealId);
            if (sanction != null)
            {
                sanction.Status = "Disbursed";
            }

            db.SaveChanges();

            // Send disbursement email to customer
            try
            {
                var deal = db.Deals
                    .Include(d => d.Loan)
                    .ThenInclude(l => l.Customer)
                    .FirstOrDefault(d => d.DealId == disbursement.LoanDealId);

                var customer = deal?.Loan?.Customer;
                if (customer != null)
                {
                    emailService.SendDisbursementEmail(customer, disbursement);
                }
            }
            catch
            {
                // Email fail hone pe disbursement save rahega
            }
        }

        public List<Disbursement> GetAll()
        {
            return db.Disbursements.ToList();
        }

        public Disbursement? GetById(int id)
        {
            return db.Disbursements.FirstOrDefault(x => x.DisbursementId == id);
        }
    }
}

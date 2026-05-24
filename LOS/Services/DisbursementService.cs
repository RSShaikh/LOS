using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class DisbursementService : IDisbursementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public DisbursementService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void CreateDisbursement(Disbursement disbursement)
        {
            disbursement.DisbursementDate = DateTime.Now;
            disbursement.DisbursementStatus = "Completed";

            _context.Disbursements.Add(disbursement);

            // Update sanction letter status
            var sanction = _context.SanctionLetters.FirstOrDefault(s => s.LoanDealId == disbursement.LoanDealId);
            if (sanction != null)
            {
                sanction.Status = "Disbursed";
            }

            _context.SaveChanges();

            // Send disbursement email to customer
            try
            {
                var deal = _context.Deals
                    .Include(d => d.Loan)
                    .ThenInclude(l => l.Customer)
                    .FirstOrDefault(d => d.DealId == disbursement.LoanDealId);

                var customer = deal?.Loan?.Customer;
                if (customer != null)
                {
                    _emailService.SendDisbursementEmail(customer, disbursement);
                }
            }
            catch
            {
                // Email fail hone pe disbursement save rahega
            }
        }

        public List<Disbursement> GetAll()
        {
            return _context.Disbursements.ToList();
        }

        public Disbursement? GetById(int id)
        {
            return _context.Disbursements.FirstOrDefault(x => x.DisbursementId == id);
        }
    }
}

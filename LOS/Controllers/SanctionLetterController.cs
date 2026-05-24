using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOS.Controllers
{
    public class SanctionLetterController : Controller
    {
        private readonly ISanctionService _service;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public SanctionLetterController(
            ISanctionService service,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _service = service;
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View(_service.GetAll());
        }

        

        public IActionResult GenerateLetter(int id)
        {
            var sanction = _service.GetById(id);

            if (sanction == null)
                return NotFound();

            var deal = _context.Deals
                .Include(d => d.Loan)
                .ThenInclude(l => l.Customer)
                .FirstOrDefault(x => x.DealId == sanction.LoanDealId);

            var customer = deal?.Loan?.Customer;

            ViewBag.CustomerName = customer != null
                ? $"{customer.FirstName} {customer.LastName}"
                : "N/A";

            return View(sanction);
        }


       

        public IActionResult GenerateDirect(int loanDealId)
        {
            var deal = _context.Deals
                .Include(d => d.Loan)
                .ThenInclude(l => l.Customer)
                .FirstOrDefault(x => x.DealId == loanDealId);

            if (deal == null || deal.Status != "Approved")
                return BadRequest("Deal not found or not approved");

            var sanction = new SanctionLetter
            {
                LoanDealId = loanDealId,
                LoanAmount = deal.ApprovedAmount,
                InterestRate = deal.InterestRate,
                TenureMonths = deal.Tenure,
                EMIAmount = _service.CalculateEMI(deal.ApprovedAmount, deal.InterestRate, deal.Tenure)
            };

            var result = _service.CreateSanction(sanction);
            // ✅ Update Deal status using the same deal object
            if (deal != null)
            {
                deal.Status = "Sanctioned";   // or "SanctionGenerated"
                _context.SaveChanges();
            }

            string customerName = $"{deal.Loan.Customer.FirstName} {deal.Loan.Customer.LastName}";
            string pdfPath = _service.GeneratePdf(result, customerName);

            try
            {
                _emailService.SendSanctionLetterEmail(deal.Loan.Customer, result, pdfPath);
            }
            catch
            {
                TempData["Warning"] = "Sanction letter created but email could not be sent.";
            }

            return RedirectToAction("GenerateLetter", new { id = result.SanctionLetterId });
        }

    }
}
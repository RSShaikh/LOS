using LOS.Data;
using LOS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOS.Controllers
{
    [Authorize]
    public class CustController : Controller
    {
        IEligibilityService CeligibilityService;
        private readonly ApplicationDbContext db;

        public CustController(IEligibilityService eligibilityService, ApplicationDbContext db)
        {
            CeligibilityService = eligibilityService;
            this.db = db;
        }

        private int GetCustomerId()
        {
            var val = User.FindFirst("CustomerId")?.Value;
            return int.TryParse(val, out int id) ? id : 0;
        }

        public IActionResult MyEligibility()
        {
            var claimValue = User.FindFirst("CustomerId")?.Value;
            if (string.IsNullOrEmpty(claimValue))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            int customerId = Convert.ToInt32(claimValue);

            var kycDocs = db.KYCDocuments
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .GroupBy(x => x.DocumentType)
                .Select(g => g.OrderByDescending(d => d.UploadDate).FirstOrDefault())
                .ToList();

            bool kycComplete = kycDocs.Any() &&
                kycDocs.All(x =>
                    !string.IsNullOrWhiteSpace(x.VerificationStatus) &&
                    x.VerificationStatus.Trim()
                        .Equals("Approved", StringComparison.OrdinalIgnoreCase));

            var cibil = db.CibilReports
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CheckDate)
                .FirstOrDefault();

            CeligibilityService.GetEligibilityResults();
            var eligibility = CeligibilityService.GetEligibilityByCustomerId(customerId);

            ViewBag.KYCIncomplete = !kycComplete;
            ViewBag.CibilNotChecked = (cibil == null || cibil.CibilScore <= 0);

            return View(eligibility);
        }


        public IActionResult MySanctionLetter()
        {
            int customerId = GetCustomerId();
            if (customerId == 0)
                return RedirectToAction("Login", "Auth");


            var sanction = db.SanctionLetters
                .Include(s => s.LoanDeals)
                    .ThenInclude(d => d.Loan)
                        .ThenInclude(l => l.Customer)
                .Where(s => s.LoanDeals.Loan.CustomerId == customerId)
                .OrderByDescending(s => s.SanctionDate)
                .FirstOrDefault();

            if (sanction != null)
            {
                var customer = sanction.LoanDeals?.Loan?.Customer;
                ViewBag.CustomerName = customer != null
                    ? $"{customer.FirstName} {customer.LastName}"
                    : "N/A";
                ViewBag.CustomerEmail = customer?.Email ?? "";
            }

            return View(sanction);
        }


        public IActionResult MyDisbursement()
        {
            int customerId = GetCustomerId();
            if (customerId == 0)
                return RedirectToAction("Login", "Auth");


            var disbursement = db.Disbursements
                .Include(d => d.LoanDeals)
                    .ThenInclude(deal => deal.Loan)
                        .ThenInclude(l => l.Customer)
                .Where(d => d.LoanDeals.Loan.CustomerId == customerId)
                .OrderByDescending(d => d.DisbursementDate)
                .FirstOrDefault();

            if (disbursement != null)
            {
                var customer = disbursement.LoanDeals?.Loan?.Customer;
                var deal = disbursement.LoanDeals;
                var loan = deal?.Loan;

                ViewBag.CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "N/A";
                ViewBag.CustomerEmail = customer?.Email ?? "";
                ViewBag.LoanType = loan?.LoanType ?? "";
                ViewBag.EMI = deal?.EMI.ToString("N2") ?? "N/A";
                ViewBag.Tenure = deal?.Tenure ?? 0;
            }

            return View(disbursement);
        }
    }
}
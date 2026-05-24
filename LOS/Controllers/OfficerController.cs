using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOS.Controllers
{
    public class OfficerController : Controller
    {
        private readonly IDealService dealService;
        private readonly IReviewService reviewService;
        private readonly ApplicationDbContext db;

        public OfficerController(IDealService dealService, IReviewService reviewService, ApplicationDbContext db)
        {
            this.dealService = dealService;
            this.reviewService = reviewService;
            this.db = db;
        }

        // Officer dashboard
        public IActionResult Dashboard()
        {
            var deals = db.Deals
                          .Include(d => d.Loan)
                          .ThenInclude(l => l.Customer)
                          .ToList();

            return View(deals);
        }

        // Profile view
        public IActionResult Profile(int customerId)
        {
            var customer = db.Customers
                             .Include(c => c.ApplyLoans)
                             .ThenInclude(l => l.Deals)
                             .FirstOrDefault(c => c.CustomerId == customerId);

            return View(customer);
        }

        [HttpPost]
        public IActionResult Approve(int dealId, decimal approvedAmount)
        {
            var deal = db.Deals
                         .Include(d => d.Loan) // eager load loan
                         .FirstOrDefault(d => d.DealId == dealId);

            if (deal != null)
            {
                // ✅ Validation check
                if (approvedAmount > deal.RequestedAmount)
                {
                    TempData["ErrorMessage"] = "Approved amount cannot exceed requested amount.";
                    return RedirectToAction("Profile", new { customerId = deal.Loan.CustomerId });
                }

                // ✅ Update deal
                deal.Status = "Approved";
                deal.ApprovedAmount = approvedAmount;

                // ✅ Recalculate EMI with approved amount
                deal.EMI = CalculateEmi(approvedAmount, deal.InterestRate, deal.Tenure);

                // ✅ Sync back to loan
                var loan = db.ApplyLoans.FirstOrDefault(l => l.LoanId == deal.LoanId);
                if (loan != null)
                {
                    loan.Status = "Approved";
                    loan.ApprovedAmount = approvedAmount;
                    loan.NewEmi = deal.EMI;
                }

                var officerId = HttpContext.Session.GetInt32("OfficerId") ?? 0;
                var officerName = HttpContext.Session.GetString("OfficerName") ?? "Unknown Officer";

                // ✅ Log review
                db.DealReviews.Add(new DealReview
                {
                    DealId = dealId,
                    OfficerId = officerId,
                    OfficerName = officerName, // hardcoded for now
                    Decision = "Approved",
                    Remarks = "Approved successfully",
                    ReviewDate = DateTime.Now
                });

                db.SaveChanges();

                return RedirectToAction("Profile", new { customerId = loan?.CustomerId });
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult Reject(int dealId, string remarks)
        {
            var deal = db.Deals
                 .Include(d => d.Loan) // eager load loan
                 .FirstOrDefault(d => d.DealId == dealId);

            if (deal != null)
            {
                deal.Status = "Rejected";

                // ✅ also update loan status for customer side
                var loan = db.ApplyLoans.FirstOrDefault(l => l.LoanId == deal.LoanId);
                if (loan != null)
                {
                    loan.Status = "Rejected";
                    loan.Remarks = remarks;
                }

                var officerId = HttpContext.Session.GetInt32("OfficerId") ?? 0;
                var officerName = HttpContext.Session.GetString("OfficerName") ?? "Unknown Officer";

                db.DealReviews.Add(new DealReview
                {
                    DealId = dealId,
                    OfficerId = officerId,
                    OfficerName = officerName,
                    Decision = "Rejected",
                    Remarks = remarks,
                    ReviewDate = DateTime.Now
                });

                db.SaveChanges();

                return RedirectToAction("Profile", new { customerId = loan?.CustomerId });
            }

            return RedirectToAction("Dashboard");
        }

        // ✅ EMI calculation helper
        private decimal CalculateEmi(decimal principal, decimal annualRate, int months)
        {
            var monthlyRate = (double)annualRate / 12 / 100;
            var P = (double)principal;
            var N = months;

            var emi = (P * monthlyRate * Math.Pow(1 + monthlyRate, N)) /
                      (Math.Pow(1 + monthlyRate, N) - 1);

            return (decimal)Math.Round(emi, 2);
        }
    }
}

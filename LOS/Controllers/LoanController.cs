using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOS.Controllers
{
    [Authorize] // ensures only logged-in users can access
    public class LoanController : Controller
    {
        private readonly IApplyLoanService loanService;
        private readonly IDealService dealService;
        private readonly IReviewService reviewService;
        private readonly ApplicationDbContext db;

        public LoanController(IApplyLoanService loanService, IDealService dealService, IReviewService reviewService, ApplicationDbContext db)
        {
            this.loanService = loanService;
            this.dealService = dealService;
            this.reviewService = reviewService;
            this.db = db;
        }

        // Helper method to get logged-in CustomerId from JWT
        private int GetLoggedInCustomerId()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            return customerIdClaim != null ? int.Parse(customerIdClaim.Value) : 0;
        }

        // Customer form
        //public IActionResult Create()
        //{
        //    var customerId = GetLoggedInCustomerId();
        //    ViewBag.CustomerId = customerId;
        //    ViewBag.LoanTypes = db.Interests.ToList();
        //    return View();
        //}

        public IActionResult Create()
        {
            var customerId = GetLoggedInCustomerId();

            //var kyc = db.KYCDocuments.FirstOrDefault(x => x.CustomerId == customerId);
            var kycDocs = db.KYCDocuments
    .Where(x => x.CustomerId == customerId && x.IsActive)
    .ToList();
            var eligibility = db.EligibilityResults.FirstOrDefault(x => x.CustomerId == customerId);

            //bool kycComplete = kyc != null && kyc.VerificationStatus == "Approved";
            // Option A: Require ALL docs approved
            bool kycComplete = kycDocs.Any() &&
                kycDocs.All(x => x.VerificationStatus.Trim()
                    .Equals("Approved", StringComparison.OrdinalIgnoreCase));

            // Option B: Allow ANY doc approved
            // bool kycComplete = kycDocs.Any(x => x.VerificationStatus.Trim()
            //     .Equals("Approved", StringComparison.OrdinalIgnoreCase));
            bool eligible = eligibility != null && eligibility.IsEligible == true;

            ViewBag.CanApplyLoan = kycComplete && eligible;
            ViewBag.KYCIncomplete = !kycComplete;
            ViewBag.NotEligible = !eligible;

            ViewBag.CustomerId = customerId;
            ViewBag.LoanTypes = db.Interests.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(ApplyLoan applyLoan)
        {
            var customerId = GetLoggedInCustomerId();
            applyLoan.CustomerId = customerId;

            var result = loanService.CreateLoanApplication(applyLoan);

            //If duplicate or invalid, return view with model so Razor can show alert
            if (result.Status == "Duplicate" || result.Status == "Invalid")
            {
                ViewBag.CustomerId = customerId;
                ViewBag.LoanTypes = db.Interests.ToList();
                ViewBag.CanApplyLoan = true; // still show form
                return View(result); // pass model back to view
            }

            // If valid, redirect to dashboard
            return RedirectToAction("Dashboard");
        }

        //public IActionResult Create(ApplyLoan applyLoan)
        //{
        //    var customerId = GetLoggedInCustomerId();
        //    applyLoan.CustomerId = customerId;

        //    loanService.CreateLoanApplication(applyLoan);
        //    return RedirectToAction("Dashboard");
        //}

        // Customer dashboard
        public IActionResult Dashboard()
        {
            var customerId = GetLoggedInCustomerId();
            var loans = loanService.GetLoansByCustomer(customerId).ToList();
            return View(loans);
        }

        // Officer dashboard (optional)
        //public IActionResult OfficerDashboard()
        //{
        //    var deals = dealService.GetAllDeals().ToList();
        //    return View(deals);
        //}

        //[HttpPost]
        //public IActionResult Review(int dealId, string decision, string remarks)
        //{
        //    var review = new DealReview
        //    {
        //        DealId = dealId,
        //        OfficerId = 1, // Replace with logged-in officer ID later
        //        Decision = decision,
        //        Remarks = remarks
        //    };
        //    reviewService.AddReview(review);
        //    return RedirectToAction("OfficerDashboard");
        //}

        [HttpGet]
        public IActionResult GetInterestRate(string loanType)
        {
            var interest = db.Interests.FirstOrDefault(i => i.LoanType == loanType);
            if (interest != null)
            {
                return Json(interest.InterestRate);
            }
            return Json(0); // fallback if not found
        }
    }
}

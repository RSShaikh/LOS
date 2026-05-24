using LOS.Interfaces;
using LOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOS.Controllers
{
    public class AdminKycController : Controller
    {
        IKycService kycService;
        IEmailService emailService;
        ApplicationDbContext db;

        public AdminKycController(IKycService service, IEmailService emailSvc, ApplicationDbContext dbContext)
        {
            kycService = service;
            emailService = emailSvc;
            db = dbContext;
        }

        public IActionResult Index()
        {
            var customers = kycService.GetAllCustomersWithKyc();

            ViewBag.TotalCustomers = customers.Count;
            ViewBag.PendingKYC = customers.Count(c =>
                c.KYCDocument != null &&
                c.KYCDocument.Any(d => d.IsActive && d.VerificationStatus == "Pending"));
            ViewBag.ApprovedKYC = customers.Count(c =>
                c.KYCDocument != null &&
                c.KYCDocument.Where(d => d.IsActive).All(d => d.VerificationStatus == "Approved") &&
                c.KYCDocument.Any(d => d.IsActive));
            ViewBag.RejectedKYC = customers.Count(c =>
                c.KYCDocument != null &&
                c.KYCDocument.Any(d => d.IsActive && d.VerificationStatus == "Rejected"));

            return View("~/Views/Admin/Kyc/Index.cshtml", customers);
        }

        public IActionResult Review(int id)
        {
            var customer = kycService.GetCustomerWithKyc(id);

            if (customer == null) return NotFound();

            return View("~/Views/Admin/Kyc/Review.cshtml", customer);
        }

        [HttpPost]
        public IActionResult ApproveDocument(int documentId, int customerId)
        {
            kycService.ApproveDocument(documentId);
            TempData["Success"] = "Document approved successfully.";
            return RedirectToAction("Review", new { id = customerId });
        }

        [HttpPost]
        public IActionResult RejectDocument(int documentId, int customerId, string rejectReason)
        {
            kycService.RejectDocument(documentId, rejectReason);

            // Send rejection email to customer
            var doc = db.KYCDocuments.FirstOrDefault(d => d.DocumentId == documentId);
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (doc != null && customer != null)
            {
                try
                {
                    emailService.SendKycRejectionEmail(customer, doc.DocumentType, rejectReason);
                }
                catch
                {
                    // Email fail hone pe bhi rejection save rahega
                }
            }

            TempData["Error"] = "Document rejected.";
            return RedirectToAction("Review", new { id = customerId });
        }
    }
}

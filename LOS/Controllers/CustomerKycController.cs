
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    [Authorize]
    public class CustomerKycController : Controller
    {
        IKycService kycService;
        IWebHostEnvironment env;

        public CustomerKycController(IKycService service, IWebHostEnvironment environment)
        {
            kycService = service;
            env = environment;
        }

        public IActionResult Index()
        {
            int customerId = GetCustomerId();
            var customer = kycService.GetCustomerWithKyc(customerId);

            if (customer == null)
                return RedirectToAction("Login", "Auth");

            return View("~/Views/Customer/Kyc/Index.cshtml", customer);
        }

        [HttpPost]
        public IActionResult Upload(string documentType, IFormFile file)
        {
            int customerId = GetCustomerId();

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction("Index");
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowed.Contains(ext))
            {
                TempData["Error"] = "Only JPG, PNG, and PDF files are allowed.";
                return RedirectToAction("Index");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "File size must not exceed 5 MB.";
                return RedirectToAction("Index");
            }

            kycService.DeactivateOldDocuments(customerId, documentType);

            var uploadFolder = Path.Combine(env.WebRootPath, "uploads", "kyc");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{customerId}_{documentType.Replace(" ", "_")}_{DateTime.Now.Ticks}{ext}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                file.CopyTo(stream);

            var doc = new KYCDocument
            {
                CustomerId = customerId,
                DocumentType = documentType,
                FilePath = $"/uploads/kyc/{fileName}",
                FileName = file.FileName,
                VerificationStatus = "Pending",
                UploadDate = DateTime.Now,
                IsActive = true
            };

            kycService.AddDocument(doc);
            TempData["KycSuccess"] = $"{documentType} uploaded successfully. Pending verification.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int documentId)
        {
            int customerId = GetCustomerId();
            var doc = kycService.GetDocumentById(documentId);

            if (doc != null && doc.CustomerId == customerId)
            {
                var fullPath = Path.Combine(env.WebRootPath, doc.FilePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                kycService.DeleteDocument(doc);
                TempData["KycSuccess"] = "Document deleted successfully.";
            }

            return RedirectToAction("Index");
        }

        private int GetCustomerId()
        {
            var idClaim = User.Claims.FirstOrDefault(x => x.Type == "CustomerId")?.Value;
            return int.TryParse(idClaim, out int id) ? id : 0;
        }
    }
}

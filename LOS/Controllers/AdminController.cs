using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOS.Controllers
{
    public class AdminController : Controller
    {
        IAdminService adminService;

        public AdminController(IAdminService service)
        {
            adminService = service;
        }

        public IActionResult Index()
        {
            var customers = adminService.GetAllCustomersWithKyc();

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

            return View(customers);
        }
    }
}

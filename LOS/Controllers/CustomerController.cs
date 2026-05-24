using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LOS.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly ApplicationDbContext _context;

        public CustomerController(ICustomerService service, ApplicationDbContext context)
        {
            customerService = service;
            _context = context;
        }

        public IActionResult Profile()
        {
            int customerId = GetCustomerId();

            var customer = _context.Customers
                .Include(c => c.CibilReport)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
                return RedirectToAction("Login", "Auth");

            var latestCibil = customer.CibilReport?
                .OrderByDescending(x => x.CheckDate)
                .FirstOrDefault();

            ViewBag.CibilScore = latestCibil?.CibilScore;

            return View(customer);
        }

        private int GetCustomerId()
        {
            var idClaim = User.Claims.FirstOrDefault(x => x.Type == "CustomerId")?.Value;
            return int.TryParse(idClaim, out int id) ? id : 0;
        }

        public IActionResult Kyc()
        {
            int customerId = GetCustomerId();

            var customer = _context.Customers
                .Include(c => c.KYCDocument)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Login", "Auth");
            }

            return View("Kyc/Index", customer);
        }
    }
}

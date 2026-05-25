using LOS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService service)
        {
            customerService = service;
        }

        private int GetCustomerId()
        {
            var idClaim = User.Claims.FirstOrDefault(x => x.Type == "CustomerId")?.Value;
            return int.TryParse(idClaim, out int id) ? id : 0;
        }

        public IActionResult Profile()
        {
            int customerId = GetCustomerId();

            var customer = customerService.GetByIdWithCibil(customerId);

            if (customer == null)
                return RedirectToAction("Login", "Auth");

            var latestCibil = customer.CibilReport?
                .OrderByDescending(x => x.CheckDate)
                .FirstOrDefault();

            ViewBag.CibilScore = latestCibil?.CibilScore;

            return View(customer);
        }

        public IActionResult Kyc()
        {
            int customerId = GetCustomerId();

            var customer = customerService.GetById(customerId);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Login", "Auth");
            }

            return View("Kyc/Index", customer);
        }
    }
}
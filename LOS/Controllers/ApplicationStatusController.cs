using LOS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOS.Controllers
{
    public class ApplicationStatusController : Controller
    {
        private readonly ITrackStatusService _service;

        public ApplicationStatusController(ITrackStatusService service)
        {
            _service = service;
        }

        public IActionResult AIndex()
        {
            // Try to read the "CustomerId" claim first
            var customerIdClaim = User.FindFirst("CustomerId")
                               ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (customerIdClaim == null)
            {
                // No claim found, redirect to login or show error
                return RedirectToAction("Login", "Customer");
            }

            int customerId = Convert.ToInt32(customerIdClaim.Value);

            var model = _service.GetCustomerTrackStatus(customerId);

            return View(model);
        }
    }
}

using LOS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOS.Controllers
{
    public class ApplicationStatusController : Controller
    {
        private readonly ITrackStatusService Tservice;

        public ApplicationStatusController(ITrackStatusService service)
        {
            Tservice = service;
        }

        public IActionResult AIndex()
        {

            var customerIdClaim = User.FindFirst("CustomerId")
                               ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (customerIdClaim == null)
            {

                return RedirectToAction("Login", "Customer");
            }

            int customerId = Convert.ToInt32(customerIdClaim.Value);

            var model = Tservice.GetCustomerTrackStatus(customerId);

            return View(model);
        }
    }
}
using LOS.Models;
using Microsoft.AspNetCore.Mvc;
using LOS.Interfaces;

namespace LOS.Controllers
{
    public class EligibilityController : Controller
    {
        private readonly IEligibilityService _service;

        public EligibilityController(IEligibilityService service)
        {
            _service = service;
        }

        // INDEX
        public IActionResult EIndex()
        {
            var data = _service.GetEligibilityResults();
            return View(data);
        }

        // APPROVE POST (directly from EIndex inline form)
        [HttpPost]
        public IActionResult Approve(EligibilityResult model)
        {
            _service.UpdateEligibility(
                model.EligibilityId,
                true,
                null
            );

            return RedirectToAction("EIndex");
        }

        // REJECT POST (from EIndex modal form)
        [HttpPost]
        public IActionResult Reject(EligibilityResult model)
        {
            _service.UpdateEligibility(
                model.EligibilityId,
                false,
                model.RejectionReason
            );

            return RedirectToAction("EIndex");
        }
    }
}

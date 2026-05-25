using LOS.Models;
using Microsoft.AspNetCore.Mvc;
using LOS.Interfaces;

namespace LOS.Controllers
{
    public class EligibilityController : Controller
    {
        private readonly IEligibilityService Eservice;

        public EligibilityController(IEligibilityService service)
        {
            Eservice = service;
        }


        public IActionResult EIndex()
        {
            var data = Eservice.GetEligibilityResults();
            return View(data);
        }


        [HttpPost]
        public IActionResult Approve(EligibilityResult model)
        {
            Eservice.UpdateEligibility(
                model.EligibilityId,
                true,
                null
            );

            return RedirectToAction("EIndex");
        }


        [HttpPost]
        public IActionResult Reject(EligibilityResult model)
        {
            Eservice.UpdateEligibility(
                model.EligibilityId,
                false,
                model.RejectionReason
            );

            return RedirectToAction("EIndex");
        }
    }
}
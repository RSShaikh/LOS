using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    public class LoanDisbursementController : Controller
    {
        private readonly IDisbursementService service;

        public LoanDisbursementController(IDisbursementService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View(service.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Disbursement disbursement)
        {
            if (!ModelState.IsValid)
                return View(disbursement);

            service.CreateDisbursement(disbursement);
            return RedirectToAction("Index");
        }
    }
}

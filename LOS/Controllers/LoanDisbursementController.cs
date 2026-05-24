using LOS.Interfaces;
using LOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    public class LoanDisbursementController : Controller
    {
        private readonly IDisbursementService _service;

        public LoanDisbursementController(IDisbursementService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.GetAll());
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

            _service.CreateDisbursement(disbursement);
            return RedirectToAction("Index");
        }
    }
}

using LOS.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    public class CibilController : Controller
    {
        private readonly ICibilService cibilService;

        public CibilController(ICibilService cibilService)
        {
            this.cibilService = cibilService;
        }

        // GET
        public async Task<IActionResult> Check()
        {
            int customerId =
                GetCurrentCustomerId();

            if (customerId == 0)
            {
                TempData["Error"] =
                    "Session expired.";

                return RedirectToAction(
                    "Login",
                    "Auth"
                );
            }

            var viewModel =
                await cibilService
                .GetCustomerWithCibilAsync(
                    customerId
                );

            if (viewModel == null)
            {
                TempData["Error"] =
                    "Customer not found.";

                return RedirectToAction(
                    "Login",
                    "Auth"
                );
            }

            return View(viewModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(
decimal bankBalance)
        {
            int customerId =
            GetCurrentCustomerId();

            if (customerId == 0)
            {
                TempData["Error"] =
                "Session expired.";

                return RedirectToAction(
                "Login",
                "Auth");
            }

            try
            {
                await cibilService
                .GenerateAndSaveCibilScoreAsync(
                    customerId,
                    bankBalance
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                ex.Message;
            }

            return RedirectToAction(
            nameof(Check));
        }

        private int GetCurrentCustomerId()
        {
            var claim =
                User
                .FindFirst(
                    "CustomerId"
                )?.Value;

            if (
                string.IsNullOrEmpty(claim)
                ||
                !int.TryParse(
                    claim,
                    out int id
                )
            )
            {
                return 0;
            }

            return id;
        }
    }
}
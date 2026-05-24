using LOS.Interfaces;
using LOS.Models;
using LOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace LOS.Controllers
{
    public class ScoreCardController : Controller
    {
        private readonly IScoreCardService score;

        public ScoreCardController(IScoreCardService score)
        {
            this.score = score;
        }

        public IActionResult Index(int customerId)
        {
            score.GenerateOrUpdateScoreCards();

            var data = score.FetchScoreCard();

            return View(data);
        }

        public IActionResult Edit(int id)
        {
            var data = score.GetScoreCardById(id);

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(ScoreCard sc)
        {
            score.EditScoreCard(sc.ScoreCardId, sc.RiskCategory);

            return RedirectToAction("Index");
        }

        

    }
}

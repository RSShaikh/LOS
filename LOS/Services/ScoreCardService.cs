using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class ScoreCardService : IScoreCardService
    {
        private readonly ApplicationDbContext db;

        public ScoreCardService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public string EditScoreCard(int id, string riskCategory)
        {
            var data = db.ScoreCards.Find(id);

            if (data != null)
            {
                if (riskCategory != "Low Risk" &&
                    riskCategory != "Medium Risk" &&
                    riskCategory != "High Risk")
                {
                    return "Please choose a valid Risk Category";
                }

                data.RiskCategory = riskCategory;

                data.IsRiskOverridden = true;

                data.ModifiedBy = "Admin";
                data.ModifiedAt = DateTime.Now;

                db.SaveChanges();

                return "Success";
            }

            return "ScoreCard not found";
        }

        public ScoreCard GetScoreCardById(int id)
        {
            return db.ScoreCards
                     .Include(x => x.Customer)
                     .FirstOrDefault(x => x.ScoreCardId == id);
        }

        public List<ScoreCard> FetchScoreCard()
        {
            var data = db.ScoreCards.Include(x => x.Customer)
                      .ThenInclude(c => c.CibilReport)
                      .ToList();
            return data;
        }
        public void GenerateOrUpdateScoreCards()
        {
            var customers = db.Customers
                .Include(x => x.CibilReport)
                .ToList();

            foreach (var customer in customers)
            {
                var eligibilityResult = db.EligibilityResults
                    .FirstOrDefault(x => x.CustomerId == customer.CustomerId);

                if (eligibilityResult == null)
                    continue;

                var scoreCard = db.ScoreCards
                    .FirstOrDefault(x => x.CustomerId == customer.CustomerId);


                if (scoreCard == null)
                {
                    scoreCard = new ScoreCard();

                    scoreCard.CustomerId = customer.CustomerId;

                    scoreCard.CreatedBy = "System";
                    scoreCard.CreatedAt = DateTime.Now;

                    db.ScoreCards.Add(scoreCard);
                }

                bool eligible = eligibilityResult.IsEligible ?? false;


                if (!eligible)
                {
                    scoreCard.EligibleLoanAmount = 0;
                    scoreCard.RiskCategory = "None";
                }
                else
                {
                    decimal salary = customer.MonthlyIncome ?? 0;

                    int cibil = customer.CibilReport?
                        .OrderByDescending(r => r.CheckDate)
                        .FirstOrDefault()?.CibilScore ?? 0;


                    if (salary > 100000)
                    {
                        if (cibil >= 800)
                            scoreCard.EligibleLoanAmount = 5000000;

                        else if (cibil >= 750)
                            scoreCard.EligibleLoanAmount = 4000000;

                        else if (cibil >= 650)
                            scoreCard.EligibleLoanAmount = 2500000;

                        else
                            scoreCard.EligibleLoanAmount = 0;
                    }



                    else if (salary >= 90000)
                    {
                        if (cibil >= 800)
                            scoreCard.EligibleLoanAmount = 4000000;

                        else if (cibil >= 750)
                            scoreCard.EligibleLoanAmount = 3000000;

                        else if (cibil >= 650)
                            scoreCard.EligibleLoanAmount = 2000000;

                        else
                            scoreCard.EligibleLoanAmount = 0;
                    }



                    else if (salary >= 70000)
                    {
                        if (cibil >= 800)
                            scoreCard.EligibleLoanAmount = 2500000;

                        else if (cibil >= 750)
                            scoreCard.EligibleLoanAmount = 2000000;

                        else if (cibil >= 650)
                            scoreCard.EligibleLoanAmount = 1000000;

                        else
                            scoreCard.EligibleLoanAmount = 0;
                    }



                    else if (salary >= 50000)
                    {
                        if (cibil >= 750)
                            scoreCard.EligibleLoanAmount = 1000000;

                        else if (cibil >= 650)
                            scoreCard.EligibleLoanAmount = 500000;

                        else
                            scoreCard.EligibleLoanAmount = 0;
                    }

                    else
                    {
                        scoreCard.EligibleLoanAmount = 0;
                    }

                    if (!scoreCard.IsRiskOverridden)
                    {
                        if (cibil > 750)
                        {
                            scoreCard.RiskCategory = "Low Risk";
                        }
                        else if (cibil > 650 && cibil <= 750)
                        {
                            scoreCard.RiskCategory = "Medium Risk";
                        }
                        else
                        {
                            scoreCard.RiskCategory = "High Risk";
                        }
                    }
                }
            }

            db.SaveChanges();
        }
    }
}

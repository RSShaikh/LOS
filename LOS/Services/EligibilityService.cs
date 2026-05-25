using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class EligibilityService : IEligibilityService
    {
        ApplicationDbContext db;

        public EligibilityService
        (
            ApplicationDbContext context
        )
        {
            db = context;
        }

        public List<EligibilityResult> GetEligibilityResults()
        {
            var customers = db.Customers
                .Include(x => x.CibilReport)
                .ToList();

            foreach (var customer in customers)
            {
                var latestCibil = customer.CibilReport?
                    .OrderByDescending(x => x.CheckDate)
                    .FirstOrDefault();

                if (latestCibil == null)
                    continue;

                var existing = db.EligibilityResults
                    .FirstOrDefault(x => x.CustomerId == customer.CustomerId);

                if (existing == null)
                {
                    existing = new EligibilityResult
                    {
                        CustomerId = customer.CustomerId
                    };
                    db.EligibilityResults.Add(existing);
                }


                existing.CibilScore = latestCibil.CibilScore;


                bool alreadyReviewed = existing.ReviewStatus == "Approved By Officer"
                                    || existing.ReviewStatus == "Rejected By Officer";

                if (!alreadyReviewed)
                {

                    existing.IsEligible = null;
                    existing.ReviewStatus = "Manual Review";
                    existing.RejectionReason = null;
                }
            }

            db.SaveChanges();

            return db.EligibilityResults
                .Include(x => x.Customer)
                .ToList();
        }




        public void UpdateEligibility
        (
            int eligibilityId,
            bool isEligible,
            string? rejectionReason
        )
        {
            var data = db.EligibilityResults
                .FirstOrDefault
                (
                    x => x.EligibilityId == eligibilityId
                );

            if (data != null)
            {
                data.IsEligible = isEligible;

                if (isEligible)
                {
                    data.ReviewStatus = "Approved By Officer";
                    data.RejectionReason = null;
                }
                else
                {
                    data.ReviewStatus = "Rejected By Officer";
                    data.RejectionReason = rejectionReason;
                }

                db.SaveChanges();
            }

        }



        public EligibilityResult GetEligibilityByCustomerId
        (
            int customerId
        )
        {
            return db.EligibilityResults
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.CustomerId == customerId);
        }

    }
}
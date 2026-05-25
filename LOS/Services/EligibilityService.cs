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


        // GET ALL ELIGIBILITY RESULTS

        //public List<EligibilityResult>
        //    GetEligibilityResults()
        //{
        //    var customers = _context.Customers
        //        .Include(x => x.CibilReport)
        //        .ToList();

        //    foreach (var customer in customers)
        //    {
        //        var latestCibil = customer.CibilReport?
        //            .OrderByDescending(x => x.CheckDate)
        //            .FirstOrDefault();

        //        if (latestCibil == null)
        //            continue;

        //        var existing = _context.EligibilityResults
        //            .FirstOrDefault
        //            (
        //                x => x.CustomerId == customer.CustomerId
        //            );

        //        if (existing == null)
        //        {
        //            var result = new EligibilityResult
        //            {
        //                CustomerId = customer.CustomerId,
        //                CibilScore = latestCibil.CibilScore
        //            };

        //            // AUTO APPROVED

        //            if (latestCibil.CibilScore > 750)
        //            {
        //                result.IsEligible = true;
        //                result.ReviewStatus = "Auto Approved";
        //            }

        //            // MANUAL REVIEW

        //            else if
        //            (
        //                latestCibil.CibilScore >= 650 &&
        //                latestCibil.CibilScore <= 749
        //            )
        //            {
        //                result.IsEligible = null;
        //                result.ReviewStatus = "Manual Review";
        //            }

        //            // AUTO REJECTED

        //            else
        //            {
        //                result.IsEligible = false;
        //                result.ReviewStatus = "Auto Rejected";
        //                result.RejectionReason = "Low CIBIL Score";
        //            }

        //            _context.EligibilityResults.Add(result);
        //        }
        //    }

        //    _context.SaveChanges();

        //    return _context.EligibilityResults
        //        .Include(x => x.Customer)
        //        .ToList();
        //}

        //public List<EligibilityResult> GetEligibilityResults()
        //{
        //    var customers = db.Customers
        //        .Include(x => x.CibilReport)
        //        .ToList();

        //    foreach (var customer in customers)
        //    {
        //        var latestCibil = customer.CibilReport?
        //            .OrderByDescending(x => x.CheckDate)
        //            .FirstOrDefault();

        //        if (latestCibil == null)
        //            continue;

        //        var existing = db.EligibilityResults
        //            .FirstOrDefault(x => x.CustomerId == customer.CustomerId);

        //        if (existing == null)
        //        {
        //            existing = new EligibilityResult
        //            {
        //                CustomerId = customer.CustomerId
        //            };
        //            db.EligibilityResults.Add(existing);
        //        }

        //        // Always update with latest CIBIL score
        //        existing.CibilScore = latestCibil.CibilScore;

        //        // Only update status if officer has NOT already reviewed it
        //        bool alreadyReviewed = existing.ReviewStatus == "Approved By Officer"
        //                            || existing.ReviewStatus == "Rejected By Officer";

        //        if (!alreadyReviewed)
        //        {
        //            // All records go to Manual Review — officer decides
        //            existing.IsEligible = null;
        //            existing.ReviewStatus = "Manual Review";
        //            existing.RejectionReason = null;
        //        }
        //    }

        //    db.SaveChanges();

        //    return db.EligibilityResults
        //        .Include(x => x.Customer)
        //        .ToList();
        //}

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

                // Always update with latest CIBIL score
                existing.CibilScore = latestCibil.CibilScore;

                // Officer review status check
                bool alreadyReviewed = existing.ReviewStatus == "Approved By Officer"
                                    || existing.ReviewStatus == "Rejected By Officer";

                if (!alreadyReviewed)
                {
                    if (latestCibil.CibilScore > 750)
                    {
                        // Auto Approved
                        existing.IsEligible = true;
                        existing.ReviewStatus = "Auto Approved";
                        existing.RejectionReason = null;
                    }
                    else if (latestCibil.CibilScore >= 650 && latestCibil.CibilScore <= 749)
                    {
                        // Manual Review
                        existing.IsEligible = null;
                        existing.ReviewStatus = "Manual Review";
                        existing.RejectionReason = null;
                    }
                    else
                    {
                        // Auto Rejected
                        existing.IsEligible = false;
                        existing.ReviewStatus = "Auto Rejected";
                        existing.RejectionReason = "Low CIBIL Score";
                    }
                }
            }

            db.SaveChanges();

            return db.EligibilityResults
                .Include(x => x.Customer)
                .ToList();
        }


        // APPROVE / REJECT

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

        // GET ELIGIBILITY BY CUSTOMER ID

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
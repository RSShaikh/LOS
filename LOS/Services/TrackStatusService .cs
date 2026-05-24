using LOS.Data;
using LOS.Interfaces;
using LOS.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class TrackStatusService : ITrackStatusService
    {
        private readonly ApplicationDbContext db;

        public TrackStatusService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public TrackStatusViewModel GetCustomerTrackStatus(int customerId)
        {
            var model = new TrackStatusViewModel();

            //var kyc = db.KYCDocuments.FirstOrDefault(x => x.CustomerId == customerId);
            var cibil = db.CibilReports.FirstOrDefault(x => x.CustomerId == customerId);
            var eligibility = db.EligibilityResults.FirstOrDefault(x => x.CustomerId == customerId);
            var scoreCard = db.ScoreCards.FirstOrDefault(x => x.CustomerId == customerId);

            var deal = db.Deals
                .Include(x => x.Loan)
                .Include(x => x.Disbursement)
                .Include(x => x.DealReviews)
                .Where(x => x.Loan.CustomerId == customerId)
                .OrderByDescending(x => x.DealId)
                .FirstOrDefault();

            // 1. KYC STATUS
            //bool kycApproved = kyc != null && kyc.VerificationStatus == "Approved";
            //bool kycRejected = kyc != null && kyc.VerificationStatus == "Rejected";

            //model.Steps.Add(new TrackStepViewModel
            //{
            //    Title = "KYC Verification",
            //    Status = kycApproved ? "Approved" : kycRejected ? "Rejected" : "Pending",
            //    Message = kycApproved
            //        ? "Your KYC documents are approved."
            //        : kycRejected
            //            ? "Your KYC documents are rejected. Please re-upload rejected documents."
            //            : "Your KYC verification is pending.",
            //    IsCompleted = kycApproved,
            //    IsRejected = kycRejected
            //});

            // 1. KYC STATUS (fetch latest active document)
            var kyc = db.KYCDocuments
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.DocumentId) // or SubmittedDate if you have it
                .FirstOrDefault();

            bool kycApproved = kyc != null &&
                kyc.VerificationStatus?.Trim().Equals("Approved", StringComparison.OrdinalIgnoreCase) == true;
            bool kycRejected = kyc != null &&
                kyc.VerificationStatus?.Trim().Equals("Rejected", StringComparison.OrdinalIgnoreCase) == true;

            model.Steps.Add(new TrackStepViewModel
            {
                Title = "KYC Verification",
                Status = kycApproved ? "Approved" : kycRejected ? "Rejected" : "Pending",
                Message = kycApproved
                    ? "Your KYC documents are approved."
                    : kycRejected
                        ? $"Your KYC documents are rejected. Reason: {kyc.RejectReason}"
                        : "Your KYC verification is pending.",
                IsCompleted = kycApproved,
                IsRejected = kycRejected
            });


            // 2. CIBIL STATUS
            model.Steps.Add(new TrackStepViewModel
            {
                Title = "CIBIL Report",
                Status = cibil != null ? "Generated" : "Pending",
                Message = cibil != null
                    ? $"Your CIBIL report is generated. Score: {cibil.CibilScore}."
                    : "Your CIBIL report is not generated yet.",
                IsCompleted = cibil != null,
                IsRejected = false
            });

            // 3. ELIGIBILITY STATUS
            bool eligibilityRejected = eligibility != null && eligibility.IsEligible == false;

            model.Steps.Add(new TrackStepViewModel
            {
                Title = "Eligibility Check",
                Status = eligibility == null ? "Pending" : eligibility.ReviewStatus ?? "Pending",
                Message = eligibility == null
                    ? "Your eligibility is not checked yet."
                    : eligibility.IsEligible == true
                        ? "You are eligible for loan."
                        : $"You are not eligible for loan. Reason: {eligibility.RejectionReason}",
                IsCompleted = eligibility != null && eligibility.IsEligible == true,
                IsRejected = eligibilityRejected
            });

            // 4. SCORECARD STATUS
            model.Steps.Add(new TrackStepViewModel
            {
                Title = "ScoreCard",
                Status = scoreCard != null ? "Generated" : "Pending",
                Message = scoreCard != null
                    ? $"ScoreCard generated. Eligible loan amount: ₹{scoreCard.EligibleLoanAmount}."
                    : "Your ScoreCard is not generated yet.",
                IsCompleted = scoreCard != null,
                IsRejected = false
            });

            // 5. LOAN + DEAL REVIEW STATUS
            if (deal == null)
            {
                model.Steps.Add(new TrackStepViewModel
                {
                    Title = "Loan Application Review",
                    Status = "Not Applied",
                    Message = "You have not applied for loan yet.",
                    IsCompleted = false,
                    IsRejected = false
                });
            }
            else
            {
                var latestReview = deal.DealReviews?
                    .OrderByDescending(x => x.ReviewDate)
                    .FirstOrDefault();

                var reviewStatus = latestReview?.Decision ?? "Pending";

                model.Steps.Add(new TrackStepViewModel
                {
                    Title = "Loan Application Review",
                    Status = reviewStatus,
                    Message = latestReview != null
                        ? $"Decision: {latestReview.Decision}. Remarks: {latestReview.Remarks}"
                        : "Your loan application is pending for officer review.",
                    IsCompleted = reviewStatus == "Approved",
                    IsRejected = reviewStatus == "Rejected"
                });
            }

            // 6. DISBURSEMENT STATUS
            model.Steps.Add(new TrackStepViewModel
            {
                Title = "Disbursement",
                Status = deal?.Disbursement?.DisbursementStatus ?? "Pending",
                Message = deal?.Disbursement != null
                    ? $"Disbursement status: {deal.Disbursement.DisbursementStatus}. Date: {deal.Disbursement.DisbursementDate:d}"
                    : "Loan amount is not disbursed yet.",
                IsCompleted = deal?.Disbursement?.DisbursementStatus == "Completed"
                           || deal?.Disbursement?.DisbursementStatus == "Disbursed",
                IsRejected = false
            });

            return model;
        }
    }
}

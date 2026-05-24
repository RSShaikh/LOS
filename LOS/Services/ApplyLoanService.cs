using LOS.Data;
using LOS.Interfaces;
using LOS.Models;

namespace LOS.Services
{
    public class ApplyLoanService : IApplyLoanService
    {

        private readonly ApplicationDbContext db;

        public ApplyLoanService(ApplicationDbContext db)
        {
            this.db = db;
        }

        //public bool CanApplyLoan(int customerId)
        //{
        //    var kyc = db.KYCDocuments
        //.FirstOrDefault(x => x.CustomerId == customerId);

        //    var eligibility = db.EligibilityResults
        //        .FirstOrDefault(x => x.CustomerId == customerId);

        //    // KYC must be approved
        //    bool kycComplete = kyc != null && kyc.VerificationStatus == "Approved";

        //    // Eligibility must be true
        //    bool eligible = eligibility != null && eligibility.IsEligible == true;

        //    return kycComplete && eligible;
        //}

        public bool CanApplyLoan(int customerId)
        {
            // Get all active KYC documents for this customer
            var kycDocs = db.KYCDocuments
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .ToList();

            // Require ALL documents approved
            bool kycComplete = kycDocs.Any() &&
                kycDocs.All(x =>
                    !string.IsNullOrWhiteSpace(x.VerificationStatus) &&
                    x.VerificationStatus.Trim()
                     .Equals("Approved", StringComparison.OrdinalIgnoreCase));

            // Eligibility check
            var eligibility = db.EligibilityResults
                .FirstOrDefault(x => x.CustomerId == customerId);

            bool eligible = eligibility != null && eligibility.IsEligible == true;

            return kycComplete && eligible;
        }


        //public ApplyLoan CreateLoanApplication(ApplyLoan applyLoan)
        //{
        //    // Hardcode CustomerId for testing
        //    //applyLoan.CustomerId = 1;

        //    // Fetch interest rate
        //    var interest = db.Interests.FirstOrDefault(i => i.LoanType == applyLoan.LoanType);
        //    if (interest != null)
        //        applyLoan.InterestRate = interest.InterestRate;


        //    if (applyLoan.RequestedAmount < 1000 || applyLoan.RequestedAmount > 10000000)
        //    {
        //        throw new ArgumentException("Requested amount must be between ₹1,000 and ₹10,000,000");
        //    }

        //    // Calculate EMI
        //    applyLoan.Emi = CalculateEmi(applyLoan.RequestedAmount, applyLoan.InterestRate, applyLoan.Tenure);


        //    applyLoan.Status = "Applied";
        //    applyLoan.CreatedDate = DateTime.Now;

        //    db.ApplyLoans.Add(applyLoan);
        //    db.SaveChanges();

        //    // Create a Deal entry linked to this loan
        //    var deal = new Deal
        //    {
        //        LoanId = applyLoan.LoanId,
        //        RequestedAmount = applyLoan.RequestedAmount, //  customer’s requested amount
        //        ApprovedAmount = 0, //  officer will set later
        //        Tenure = applyLoan.Tenure,
        //        InterestRate = applyLoan.InterestRate,
        //        EMI = applyLoan.Emi,
        //        Status = "Pending" // officer will update later
        //    };

        //    db.Deals.Add(deal);
        //    db.SaveChanges();


        //    return applyLoan;
        //}

        public ApplyLoan CreateLoanApplication(ApplyLoan applyLoan)
        {
            // ✅ Prevent duplicate loan type applications for the same customer
            var existingLoan = db.ApplyLoans
                .FirstOrDefault(l => l.CustomerId == applyLoan.CustomerId
                                  && l.LoanType == applyLoan.LoanType
                                  && (l.Status == "Applied"
                                      || l.Status == "Approved"
                                      || l.Status == "Sanctioned"
                                      || l.Status == "Disbursed"));

            if (existingLoan != null)
            {
                // Instead of throwing, set a friendly message
                applyLoan.Remarks = $"You already have an active {applyLoan.LoanType} loan application.";
                applyLoan.Status = "Duplicate";

                return applyLoan; // return without saving
            }

            // Fetch interest rate
            var interest = db.Interests.FirstOrDefault(i => i.LoanType == applyLoan.LoanType);
            if (interest != null)
                applyLoan.InterestRate = interest.InterestRate;

            if (applyLoan.RequestedAmount < 1000 || applyLoan.RequestedAmount > 10000000)
            {
                throw new ArgumentException("Requested amount must be between ₹1,000 and ₹10,000,000");
            }

            // Calculate EMI
            applyLoan.Emi = CalculateEmi(applyLoan.RequestedAmount, applyLoan.InterestRate, applyLoan.Tenure);

            applyLoan.Status = "Applied";
            applyLoan.CreatedDate = DateTime.Now;

            db.ApplyLoans.Add(applyLoan);
            db.SaveChanges();

            // Create a Deal entry linked to this loan
            var deal = new Deal
            {
                LoanId = applyLoan.LoanId,
                RequestedAmount = applyLoan.RequestedAmount,
                ApprovedAmount = 0,
                Tenure = applyLoan.Tenure,
                InterestRate = applyLoan.InterestRate,
                EMI = applyLoan.Emi,
                Status = "Pending"
            };

            db.Deals.Add(deal);
            db.SaveChanges();

            return applyLoan;
        }


        public ApplyLoan GetLoanApplicationById(int loanId) =>
             db.ApplyLoans.FirstOrDefault(l => l.LoanId == loanId);

        public IQueryable<ApplyLoan> GetLoansByCustomer(int customerId) =>
            db.ApplyLoans.Where(l => l.CustomerId == customerId);

        private decimal CalculateEmi(decimal principal, decimal annualRate, int months)
        {
            var monthlyRate = (double)annualRate / 12 / 100;
            var P = (double)principal;
            var N = months;

            var emi = (P * monthlyRate * Math.Pow(1 + monthlyRate, N)) /
                      (Math.Pow(1 + monthlyRate, N) - 1);

            return (decimal)Math.Round(emi, 2);
        }
    }
}

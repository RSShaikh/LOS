using LOS.Models;

namespace LOS.Interfaces
{
    public interface IApplyLoanService
    {
        ApplyLoan CreateLoanApplication(ApplyLoan applyLoan);
        ApplyLoan GetLoanApplicationById(int loanId);
        IQueryable<ApplyLoan> GetLoansByCustomer(int customerId);

        public bool CanApplyLoan(int customerId);

    }
}

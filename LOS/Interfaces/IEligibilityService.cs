using LOS.Models;

namespace LOS.Interfaces
{
    public interface IEligibilityService
    {
        List<EligibilityResult> GetEligibilityResults();

        void UpdateEligibility
        (
            int eligibilityId,
            bool isEligible,
            string? rejectionReason
        );

        EligibilityResult GetEligibilityByCustomerId(int customerId);
    }
}
using LOS.Models;

namespace LOS.Interfaces
{
    public interface IDisbursementService
    {
        void CreateDisbursement(Disbursement disbursement);

        List<Disbursement> GetAll();

        Disbursement? GetById(int id);
    }
}

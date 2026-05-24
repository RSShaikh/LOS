using LOS.Models;

namespace LOS.Interfaces
{
    public interface ISanctionService
    {
        SanctionLetter CreateSanction(SanctionLetter sanction);
        List<SanctionLetter> GetAll();
        SanctionLetter? GetById(int id);
        decimal CalculateEMI(decimal amount, decimal rate, int months);
        string GeneratePdf(SanctionLetter sanction, string customerName);
    }
}

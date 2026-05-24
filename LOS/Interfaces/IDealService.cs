using LOS.Models;

namespace  LOS.Interfaces
{
    public interface IDealService
    {
        Deal CreateDeal(Deal deal);
        IQueryable<Deal> GetAllDeals();
    }
}

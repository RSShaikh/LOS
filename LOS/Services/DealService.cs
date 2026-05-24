using LOS.Data;
using LOS.Interfaces;
using LOS.Models;

namespace LOS.Services
{
    public class DealService : IDealService
    {
        private readonly ApplicationDbContext db;

        public DealService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public Deal CreateDeal(Deal deal)
        {
            db.Deals.Add(deal);
            db.SaveChanges();
            return deal;
        }

        public IQueryable<Deal> GetAllDeals() => db.Deals;
    }
}

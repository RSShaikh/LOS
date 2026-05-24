using LOS.Models;

namespace LOS.Interfaces
{
    public interface IReviewService
    {
        DealReview AddReview(DealReview review);
        IQueryable<DealReview> GetReviewsByDeal(int dealId);
    }
}

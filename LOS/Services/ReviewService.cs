using LOS.Data;
using LOS.Interfaces;
using LOS.Models;

namespace LOS.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext db;

        public ReviewService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public DealReview AddReview(DealReview review)
        {
            db.DealReviews.Add(review);
            db.SaveChanges();
            return review;
        }

        public IQueryable<DealReview> GetReviewsByDeal(int dealId) =>
            db.DealReviews.Where(r => r.DealId == dealId);
    }
}

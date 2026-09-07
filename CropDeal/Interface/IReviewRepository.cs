namespace CropDeal.Interface;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
        Task<IEnumerable<Review>> GetReviewsByDealerIdAsync(Guid dealerId);
        Task<IEnumerable<Review>> GetReviewsByFarmerIdAsync(Guid farmerId);
        Task<(double AverageRating, IEnumerable<Review> Reviews)> GetFarmerReviewsWithSummaryAsync(Guid farmerId);
        Task<Review> AddReviewAsync(ReviewDto dto, Guid dealerId);
        Task<bool> DeleteAsync(Guid reviewId);
        Task<object> GetFarmerReviewReportAsync(Guid farmerId);
}

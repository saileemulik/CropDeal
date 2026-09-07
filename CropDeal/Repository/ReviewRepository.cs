namespace CropDeal.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly CropDealDBContext _context;

    public ReviewRepository(CropDealDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _context.Reviews.Include(r => r.Dealer).Include(r => r.Farmer).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByDealerIdAsync(Guid dealerId)
    {
        return await _context.Reviews.Where(r => r.DealerId == dealerId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByFarmerIdAsync(Guid farmerId)
    {
        return await _context.Reviews.Where(r => r.FarmerId == farmerId).ToListAsync();
    }

    public async Task<(double AverageRating, IEnumerable<Review> Reviews)> GetFarmerReviewsWithSummaryAsync(Guid farmerId)
    {
        var reviews = await _context.Reviews.Where(r => r.FarmerId == farmerId).ToListAsync();
        double average = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        return (average, reviews);
    }

    public async Task<Review> AddReviewAsync(ReviewDto dto, Guid dealerId)
    {
        var dealerExists = await _context.Users.AnyAsync(u => u.Id == dealerId && u.Role == UserRole.Dealer);

        if (!dealerExists)
        {
            throw new Exception("Invalid DealerId. Dealer does not exist.");
        }

        var farmerReview = await _context.Reviews.AnyAsync(r => r.DealerId == dealerId && r.FarmerId == dto.FarmerId && r.TransactionId == dto.TransactionId);
        if(farmerReview)
        {
            throw new Exception($"Dealer has already rated this farmer for this transaction.");
        }
        var review = new Review
        {
            Id = Guid.NewGuid(),
            DealerId = dealerId,
            FarmerId = dto.FarmerId,
            TransactionId = dto.TransactionId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        var farmer = await _context.Users.FindAsync(dto.FarmerId);
        if (farmer == null)
        {
            throw new Exception("Invalid FarmerId. Farmer does not exist.");
        }
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        
        var reviews = await _context.Reviews.Where(r => r.FarmerId == dto.FarmerId).ToListAsync();
        farmer.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        _context.Users.Update(farmer);
        await _context.SaveChangesAsync();



        return review;
    }

    public async Task<bool> DeleteAsync(Guid reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return false;
        }
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object> GetFarmerReviewReportAsync(Guid farmerId)
    {
        var reviews = await _context.Reviews.Where(r => r.FarmerId == farmerId).ToListAsync();
        return new
        {
            TotalReviews = reviews.Count,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
            Reviews = reviews
        };
    }
}

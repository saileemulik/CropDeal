namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepo;
    private readonly CropDealDBContext _context;
     private readonly IEmailServiceRepository _emailService;

    public ReviewController(IReviewRepository reviewRepo,CropDealDBContext context, IEmailServiceRepository emailService)
    {
        _reviewRepo = reviewRepo;
         _context = context;
        _emailService = emailService;
    }

    // Admin: Get all reviews
    [HttpGet("Admin/GetAllReviews")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewRepo.GetAllAsync();
        return Ok(reviews);
    }

    // Admin: Get reviews for specific dealer
    [HttpGet("Admin/{farmerId}/GetFarmerReviewReport")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDealerReviewReport(Guid farmerId)
    {
        var report = await _reviewRepo.GetFarmerReviewReportAsync(farmerId);
        return Ok(report);
    }

    // Admin: Delete a review
    [HttpDelete("Admin/DeletReview/{reviewId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var success = await _reviewRepo.DeleteAsync(reviewId);
        return success ? NoContent() : NotFound();
    }

    // Dealer: Create a new review
  [HttpPost("Dealer/AddReview")]
[Authorize(Roles = "Dealer")]
public async Task<IActionResult> AddReview([FromBody] ReviewDto dto)
{
    try
    {
        var dealerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ✅ Save review using your repo
        var result = await _reviewRepo.AddReviewAsync(dto, Guid.Parse(dealerId));

        // ✅ Get Farmer from FarmerId in DTO
        var farmer = await _context.Users.FindAsync(dto.FarmerId);
        if (farmer != null)
        {
            var message = $"You have received a new review from a dealer for your recent transaction.";

            // ✅ Save notification
            var notification = new Notification
            {
                UserId = farmer.Id,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(notification);

            // ✅ Send email if farmer has one
            if (!string.IsNullOrEmpty(farmer.Email))
            {
                await _emailService.SendEmailAsync(
                    farmer.Email,
                    "New Review Received",
                    message
                );
            }

            await _context.SaveChangesAsync(); // Save notification
        }

        return Ok(result);
    }
    catch (Exception e)
    {
        return BadRequest(new { e.Message });
    }
}



    // Dealer: View their own submitted reviews
    [HttpGet("Dealer/GetOwnReviews")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> GetOwnReviews()
    {
        var dealerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reviews = await _reviewRepo.GetReviewsByDealerIdAsync(Guid.Parse(dealerId));
        return Ok(reviews);
    }

    // Farmer: View specific dealer reviews
    // [HttpGet("Farmer/GetReviewsOfDealer/{dealerId}")]
    // [Authorize(Roles = "Farmer")]
    // public async Task<IActionResult> GetReviewsOfDealer(Guid dealerId)
    // {
    //     var reviews = await _reviewRepo.GetReviewsByDealerIdAsync(dealerId);
    //     return Ok(reviews);
    // }

    // Farmer: View reviews received
    [HttpGet("Farmer/ReceivedReviews")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> GetReceivedReviews()
    {
        var farmerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(farmerIdString, out var farmerId))
        {
            return BadRequest("Invalid farmer ID.");
        }

        var (averageRating, reviews) = await _reviewRepo.GetFarmerReviewsWithSummaryAsync(farmerId);

        return Ok(new { averageRating, reviews });
    }

}

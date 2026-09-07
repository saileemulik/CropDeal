namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly CropDealDBContext _context;

    public SubscriptionController(ISubscriptionRepository subscriptionRepo, CropDealDBContext context)
    {
        _subscriptionRepo = subscriptionRepo;
         _context = context;
    }

    [Authorize(Roles = "Dealer")]
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscriptionDto dto)
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (dealerId == null)
        {
            return Unauthorized();
        }
        try
        {
            var subscription = await _subscriptionRepo.AddSubscriptionAsync(dealerId, dto.CropListingId ?? Guid.Empty);
            return Ok(new { subscription });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Dealer")]
    [HttpGet]
    public async Task<IActionResult> GetMySubscriptions()
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (dealerId == null)
        {
            return Unauthorized();
        }
        try
        {
            var subscriptions = await _subscriptionRepo.GetSubscriptionsByDealerIdAsync(dealerId);
            return Ok(subscriptions);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Dealer")]
    [HttpDelete("{cropListingId}")]
    public async Task<IActionResult> Unsubscribe(Guid cropListingId)
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _subscriptionRepo.DeleteSubscriptionAsync(dealerId, cropListingId);
        if (!result)
        {
            return NotFound(new { message = "Subscription not found" });
        }
        return Ok(new { message = "Unsubscribed successfully" });
    }


    [Authorize(Roles = "Dealer")]
    [HttpPost("subscribe-cropname")]
    public async Task<IActionResult> SubscribeCropName([FromBody] CropDto dto)
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (dealerId == null)
        {
            return Unauthorized();
        }
        try
        {
            var subscription = await _subscriptionRepo.AddCropNameSubscriptionAsync(dealerId, dto.CropName);
            return Ok(subscription);

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Dealer")]
    [HttpDelete("unsubscribe-cropname")]
    public async Task<IActionResult> UnsubscribeCropName([FromQuery] string cropName)
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // from JWT or session

        var success = await _subscriptionRepo.DeleteCropNameSubscriptionAsync(dealerId, cropName);

        if (!success)
            return NotFound(new { message = "No subscription found for the specified crop name." });

        return Ok(new { message = "Successfully unsubscribed from the crop." });

    }


    [Authorize(Roles = "Dealer")]
    [HttpGet("dealer-cropname-subscriptions")]
    public async Task<IActionResult> GetCropNameSubscriptions()
    {
        var dealerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine("Dealer ID from token: " + dealerIdClaim);

        if (string.IsNullOrEmpty(dealerIdClaim))
        {
            Console.WriteLine("Dealer ID is null or empty.");
            return Unauthorized();
        }

        var dealerId = Guid.Parse(dealerIdClaim);
        var subscriptions = await _subscriptionRepo.GetCropNameSubscriptionsByDealerAsync(dealerId);

        Console.WriteLine("Subscriptions fetched: " + string.Join(", ", subscriptions ?? new List<string>()));
        return Ok(subscriptions);
    }


    [Authorize(Roles = "Farmer")]
    [HttpGet("Farmer/SubscribedListings")]
    public async Task<IActionResult> GetSubscribedListingsForFarmer()
    {
        try
        {
            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (farmerId == null)
            {
                return Unauthorized("Invalid or missing user ID.");
            }

            var subscribedListings = await _subscriptionRepo.GetSubscribedListingsForFarmerAsync(farmerId);
            return Ok(new { subscribedListings });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while fetching subscribed listings.");
        }
    }

    [Authorize(Roles = "Dealer")]
    [HttpGet("matched-crop-listings")]
    public async Task<IActionResult> GetMatchingCropListingsForDealer()
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var cropNames = await _subscriptionRepo.GetCropNameSubscriptionsByDealerAsync(dealerId);

        if (cropNames == null || !cropNames.Any())
        {
            return Ok(new List<CropListing>()); // No subscriptions
        }

        // Match by crop name or type
        var matchedListings = await _context.CropListings
            .Include(cl => cl.Crop)
            .Where(cl => cropNames.Contains(cl.Crop.Name.ToLower()) ||
                        cropNames.Contains((!string.IsNullOrEmpty(cl.Crop.CustomType) ? cl.Crop.CustomType : cl.Crop.Type.ToString()).ToLower()))
            .ToListAsync();

        return Ok(matchedListings);
    }

}

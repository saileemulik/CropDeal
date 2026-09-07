namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CropListingController : ControllerBase
{
    private readonly ICropListingRepository _cropRepo;
    private readonly CropDealDBContext _context;
private readonly IEmailServiceRepository _emailService;

public CropListingController(ICropListingRepository cropRepo, CropDealDBContext context, IEmailServiceRepository emailService)
{
    _cropRepo = cropRepo;
    _context = context;
    _emailService = emailService;
}


    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllCropListings()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (userRole == "Admin")
        {
            var listings = await _cropRepo.GetAllListingsForAdminAsync();
            return Ok(listings);
        }
        else if (userRole == "Dealer")
        {
            var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var listings = await _cropRepo.GetAllCropsForDealerAsync(dealerId);
            return Ok(listings);
        }
        else
        {
            // For Farmers and others, return empty or basic listings
            return Ok(new List<object>());
        }
    }



    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (dealerId == null)
        {
            return BadRequest();
        }
        var crops = await _cropRepo.GetAllCropsForDealerAsync(dealerId);
        if (crops != null)
        {
            return Ok(crops);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var crop = await _cropRepo.GetCropByIdAsync(id);
        if (crop != null)
        {
            return Ok(new { crop });
        }
        return BadRequest(new { message = "Failed to get listings" });
    }


    [HttpGet("Dealer/{dealerId}")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> GetListingsByDealerLocation(Guid dealerId)
    {
        var listings = await _cropRepo.GetAllCropsForDealerAsync(dealerId);
        if (listings == null || !listings.Any())
        {
            return NotFound(new { message = "No crop listings found based on your location." });
        }
        return Ok(new { listings });
    }


    [HttpPost]
[Authorize(Roles = "Farmer")]
public async Task<IActionResult> AddCrop([FromBody] CropListing crop)
{
    byte[] imageBytes = null;
    if (!string.IsNullOrEmpty(crop.ImageBase64))
    {
        try
        {
            crop.ImageUrl = Convert.FromBase64String(crop.ImageBase64);
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Invalid image format.", result = false });
        }
    }

    crop.CreatedAt = DateTime.UtcNow;
    crop.UpdatedAt = DateTime.UtcNow;

    var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // Add logging here to check farmerId value before calling repository
    Console.WriteLine($"AddCrop called with FarmerId: {farmerId}");

    if (farmerId == Guid.Empty)
    {
        Console.WriteLine($"AddCrop: Invalid or empty farmerId");
        return Unauthorized("Invalid farmer ID.");
    }

    // Optional: check if farmer exists here (just logging)
    var farmerExists = await _context.Users.AnyAsync(u => u.Id == farmerId);
    Console.WriteLine($"Farmer exists in DB? {farmerExists}");

    if (!farmerExists)
    {
        Console.WriteLine($"AddCrop: Farmer with ID {farmerId} not found in database.");
        return BadRequest("Farmer does not exist.");
    }

    var created = await _cropRepo.AddCropAsync(farmerId, crop);
    if (created == null)
    {
        return BadRequest("Invalid crop selected or error creating listing");
    }

    await NotifyCropNameSubscribers(created);
    return Ok(new { message = "Crop Details added successfully" });
}



    private async Task NotifyCropNameSubscribers(CropListing listing)
{
    var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == listing.CropId);

    var cropName = crop.Name.ToLower();
    var cropType = string.IsNullOrWhiteSpace(crop.CustomType)
        ? crop.Type.ToString().ToLower()
        : crop.CustomType.ToLower();

    var matchedSubscribers = await _context.Subscriptions
        .Where(s => s.CropName != null &&
            (s.CropName.ToLower() == cropName || s.CropName.ToLower() == cropType))
        .ToListAsync();

    foreach (var sub in matchedSubscribers)
    {
        var message = $"New crop listed: {crop.Name} ({(string.IsNullOrEmpty(crop.CustomType) ? crop.Type : crop.CustomType)})";

        // Save notification
        var notification = new Notification
        {
            UserId = sub.DealerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);

        // Send email
        var dealer = await _context.Users.FindAsync(sub.DealerId);
        if (dealer != null && !string.IsNullOrEmpty(dealer.Email))
        {
            await _emailService.SendEmailAsync(dealer.Email, "New Crop Listed", message);
        }
    }

    await _context.SaveChangesAsync();
}



    [HttpPut("{id}")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> UpdateCrop(Guid id, [FromBody] CropListing crop)
    {
        var existing = await _cropRepo.GetCropByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (existing.FarmerId != userId)
        {
            return BadRequest();
        }

        existing.Unit = crop.Unit;
        existing.Location = crop.Location;
        existing.Description = crop.Description;
        existing.PricePerKg = crop.PricePerKg;
        existing.Quantity = crop.Quantity;
        existing.Status = crop.Status;
        existing.ImageUrl = crop.ImageUrl;
        await _cropRepo.UpdateCropAsync(existing);
        return Ok(new { message = "Crop Details updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> DeleteCrop(Guid id)
    {
        var crop = await _cropRepo.GetCropByIdAsync(id);
        if (crop == null)
        {
            return NotFound();
        }
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (crop.FarmerId != userId)
        {
            return BadRequest();
        }
        crop.UpdatedAt = DateTime.UtcNow;
        await _cropRepo.DeleteCropAsync(id);
        return Ok(new { message = "Crop Listing Deleted Successfully" });
    }

    [HttpGet("MyListings")]
    [Authorize(Roles = "Farmer")]
    public IActionResult GetMyListings()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var myListings = _cropRepo.GetListingsByUserId(userId);
        return Ok(myListings);
    }

    [HttpPost("upload-with-file")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> AddCropWithFile([FromForm] CropListingFileDto cropDto, IFormFile? imageFile)
    {
        var crop = new CropListing
        {
            CropId = cropDto.CropId,
            PricePerKg = cropDto.PricePerKg,
            Quantity = cropDto.Quantity,
            Unit = cropDto.Unit,
            Location = cropDto.Location,
            Status = cropDto.Status,
            Description = cropDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (imageFile != null)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            crop.ImageUrl = memoryStream.ToArray();
        }

        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var farmerExists = await _context.Users.AnyAsync(u => u.Id == farmerId);
        if (!farmerExists)
        {
            return BadRequest("Farmer does not exist.");
        }

        var created = await _cropRepo.AddCropAsync(farmerId, crop);
        if (created == null)
        {
            return BadRequest("Invalid crop selected or error creating listing");
        }

        await NotifyCropNameSubscribers(created);
        return Ok(new { message = "Crop Details added successfully with file upload" });
    }

}

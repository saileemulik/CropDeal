namespace CropDeal.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CropController : ControllerBase
{
    private readonly ICropRepository _cropRepository;
    private readonly CropDealDBContext _context;
    private readonly IEmailServiceRepository _emailService;

    public CropController(ICropRepository cropRepository, CropDealDBContext context, IEmailServiceRepository emailService)
    {
        _cropRepository = cropRepository;
        _context = context;
         _emailService = emailService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllCrops()
    {
        var crops = await _cropRepository.GetAllAsync();
        return Ok(crops);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCropById(Guid id)
    {
        var crop = await _cropRepository.GetByIdAsync(id);
        if (crop == null)
        {
            return NotFound();
        }
        return Ok(crop);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddCrop([FromBody] Crop crop, [FromQuery] CropTypeEnum type)
    {
        var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (adminId == null)
        {
            return Unauthorized("Admin not authenticated.");
        }


        crop.AdminId = adminId;

        crop.CreatedAt = DateTime.UtcNow;
        crop.UpdatedAt = DateTime.UtcNow;

        await _cropRepository.AddAsync(crop, type);
        return Ok(new { message = "Crop Added Successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCrop(Guid id, [FromBody] Crop crop, [FromQuery] CropTypeEnum type)
    {
        if (id != crop.Id)
        {
            return BadRequest();

        }
        crop.UpdatedAt = DateTime.UtcNow;
        await _cropRepository.UpdateAsync(crop, type);
        return Ok(new { message = "Crop Updated Successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCrop(Guid id)
    {
        await _cropRepository.DeleteAsync(id);
        return Ok(new { message = "Crop Deleted Successfully" });
    }


   [HttpPost("CropRequest")]
[Authorize(Roles = "Farmer")]
public async Task<IActionResult> RequestCrop([FromBody] CropRequestDto dto)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Console.WriteLine("Submitted crop type: " + dto.Type);

    var cropRequest = new CropRequest
    {
        Id = Guid.NewGuid(),
        CropName = dto.CropName,
        Description = dto.Description,
        Type = dto.Type,
        FarmerId = Guid.Parse(userId),
        Status = RequestStatus.Pending
    };

    await _context.CropRequests.AddAsync(cropRequest);
    await _context.SaveChangesAsync();

    // ✅ Notify Admin(s)
    var adminUsers = await _context.Users
        .Where(u => u.Role == UserRole.Admin)
        .ToListAsync();

    foreach (var admin in adminUsers)
    {
        var message = $"New crop request for '{dto.CropName}' has been submitted by a farmer.";

        await NotificationController.CreateNotificationAsync(_context, admin.Id, message);

        if (!string.IsNullOrEmpty(admin.Email))
        {
            await _emailService.SendEmailAsync(
                admin.Email,
                "New Crop Request Submitted",
                message
            );
        }
    }

    return Ok(new { message = "Crop request submitted successfully!" });
}


    [HttpGet("GetCropRequests")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingCropRequests()
    {
        // Only return pending requests for admin view
        var pendingRequests = await _context.CropRequests
            .Where(r => r.Status == RequestStatus.Pending)
            .Select(r => new
            {
                r.Id,
                r.CropName,
                r.Type,
                r.Description,
                r.Status,
                r.FarmerId
            })
            .ToListAsync();

        return Ok(pendingRequests);
    }


   [HttpPost("ApproveRequest/{requestId}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> ApproveCropRequest(
    Guid requestId,
    [FromQuery] RequestStatus status,
    [FromBody] CropRequestDto dto
)
{
    var request = await _context.CropRequests.FindAsync(requestId);

    if (request == null || request.Status != RequestStatus.Pending)
        return NotFound(new { message = "Crop request not found or not in pending state." });

    request.Status = status;
    Console.WriteLine($"Type received: {dto.Type}");

    var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    if (status == RequestStatus.Approved)
    {
        bool isValidEnum = Enum.TryParse<CropTypeEnum>(dto.Type, true, out var parsedEnum);

        var crop = new Crop
        {
            Id = Guid.NewGuid(),
            Name = request.CropName,
            AdminId = adminId,
            Type = isValidEnum ? parsedEnum : CropTypeEnum.Grain,
            CustomType = dto.Type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Crops.AddAsync(crop);
    }

    _context.CropRequests.Update(request);
    await _context.SaveChangesAsync();

    // ✅ Notify the farmer
    var farmer = await _context.Users.FindAsync(request.FarmerId);
    if (farmer != null)
    {
        var message = status == RequestStatus.Approved
            ? $"Your crop request for '{request.CropName}' has been approved and added to the crop list."
            : $"Your crop request for '{request.CropName}' has been rejected by the admin.";

        await NotificationController.CreateNotificationAsync(_context, farmer.Id, message);

        if (!string.IsNullOrEmpty(farmer.Email))
        {
            await _emailService.SendEmailAsync(
                farmer.Email,
                "Crop Request " + (status == RequestStatus.Approved ? "Approved" : "Rejected"),
                message
            );
        }

    }

    return Ok(new
    {
        message = status == RequestStatus.Approved
            ? "Crop request approved and crop added to crop list."
            : "Crop request rejected."
    });
}

    [HttpGet("MyCropRequests")]
    [Authorize(Roles = "Farmer,Admin")]
    public async Task<IActionResult> GetMyCropRequests()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        IQueryable<CropRequest> query = _context.CropRequests;
        
        if (userRole == "Farmer")
        {
            query = query.Where(r => r.FarmerId == userId);
        }
        // Admin can see all requests

        var requests = await query
            .Select(r => new
            {
                r.Id,
                r.CropName,
                r.Type,
                r.Status,
                r.RequestedAt,
                r.FarmerId
            })
            .ToListAsync();

        return Ok(requests);
    }
[Authorize]
[HttpGet("search")]
public async Task<IActionResult> SearchCropsByName([FromQuery] string name)
{
    var crops = await _cropRepository.SearchByNameAsync(name);
    return Ok(crops);
}

    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new { message = "Authentication working", userId, userRole, allClaims });
    }

}

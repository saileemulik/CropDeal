namespace CropDeal.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _adminRepository;
    private readonly CropDealDBContext _context;
    private readonly ICropListingRepository _cropRepo;
     private readonly IEmailServiceRepository _emailService;
     

    public AdminController(IAdminRepository adminRepository, CropDealDBContext context, ICropListingRepository cropRepo, IEmailServiceRepository emailService)
    {
        _adminRepository = adminRepository;
        _context = context;
        _cropRepo = cropRepo;
        _emailService = emailService;
    }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminRepository.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPut("EditUser/{id}")]
public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateDto user)
{
    var result = await _adminRepository.UpdateUserEntityAsync(id, user); 

    if (result == null)
    {
        return NotFound(new { message = "User not found" });
    }

    // ✅ Fetch updated user from DB
    var updatedUser = await _context.Users.FindAsync(id);
    if (updatedUser != null)
    {
        var message = "Your profile has been updated by the admin.";

        // ✅ Save Notification
        var notification = new Notification
        {
            UserId = updatedUser.Id,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);

        // ✅ Send Email if email exists
        if (!string.IsNullOrEmpty(updatedUser.Email))
        {
            await _emailService.SendEmailAsync(
                updatedUser.Email,
                "Profile Updated by Admin",
                message
            );
        }

        await _context.SaveChangesAsync(); // Save notification
    }

    return Ok(result);
}



    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await _adminRepository.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound("User not found or already deleted");
        }
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpGet("DashboardStats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var farmers = await _context.Users.CountAsync(u => u.Role == UserRole.Farmer);
        var dealers = await _context.Users.CountAsync(u => u.Role == UserRole.Dealer);
        var activeUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Active);
        var totalCrops = await _context.Crops.CountAsync();
        var totalListings = await _context.CropListings.CountAsync();
        var activeListings = await _context.CropListings.CountAsync(cl => cl.Status == CropAvailability.Available);
        var totalTransactions = await _context.Transactions.CountAsync();
        var newCropRequest = await _context.CropRequests.CountAsync(r => r.Status == RequestStatus.Pending);
        var approvedRequest = await _context.CropRequests.CountAsync(r => r.Status == RequestStatus.Approved);
        var rejectedRequest = await _context.CropRequests.CountAsync(r => r.Status == RequestStatus.Rejected);

        var transactionsPerMonth = await _context.Transactions
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new
            {
                Month = $"{g.Key.Month}/{g.Key.Year}",
                Count = g.Count()
            }).ToListAsync();

        return Ok(new
        {
            totalUsers,
            farmers,
            dealers,
            activeUsers,
            totalCrops,
            totalListings,
            activeListings,
            totalTransactions,
            approvedRequest,
            rejectedRequest,
            newCropRequest,
            transactionsPerMonth
        });
    }

}

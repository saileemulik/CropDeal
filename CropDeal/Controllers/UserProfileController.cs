namespace CropDeal.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserRepository _profileRepository;
    private readonly UserManager<User> _userManager;

    public UserProfileController(IUserRepository profileRepository, UserManager<User> userManager)
    {
        _profileRepository = profileRepository;
        _userManager = userManager;
    }

    
    [HttpGet("UserProfile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var profile = await _profileRepository.GetUserByIdAsync(Guid.Parse(userId));
        if (profile == null)
        {
            return NotFound("User not found");
        }

        return Ok(profile);
    }

    [HttpPut("UpdateUserProfile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto user)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        var profile = await _profileRepository.GetUserByIdAsModelAsync(Guid.Parse(userId));
        if (profile == null)
        {
            return NotFound();
        }

        profile.Name = user.Name;
        profile.Email = user.Email;
        profile.PhoneNumber = user.PhoneNumber;
        profile.Location = user.Location;
        // profile.BankAccountId = user.BankAccountId.Value;
        profile.UpdatedAt = DateTime.UtcNow;
        await _profileRepository.UpdateUserAsync(profile);
       return Ok(new { message = "Profile Updated Successfully" });
    }
}

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.Data;

namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    //Dependency Injection
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailServiceRepository _emailService;
    private readonly IOtpService _otpService;
    private readonly CropDealDBContext _context;

    public AuthController(UserManager<User> userManager, CropDealDBContext context, IOtpService otpService, IEmailServiceRepository emailService, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;
        _otpService = otpService;
        _context = context;
    }



    //SignUp
    [HttpPost("signup")]
    public async Task<ActionResult> SignUp([FromBody] SignUpDto dto, [FromQuery] UserRole role)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Location = dto.Location,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var roleExists = await _roleManager.RoleExistsAsync(role.ToString());
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
        }

        await _userManager.AddToRoleAsync(user, role.ToString());

        return Ok(new
        {
            message = "User registered successfully",
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.PhoneNumber,
                user.Location,
                Role = role.ToString(),
                user.CreatedAt
            }
        });
    }


    //SignIn
    [HttpPost("signin")]
    public async Task<ActionResult<string>> SignIn([FromBody] SignInDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        if (user.Status == UserStatus.Inactive && !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return Unauthorized("Your account is inactive. Please contact support.");
        }
        var userId = await _userManager.GetUserIdAsync(user);

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid email or password.");
        }

        var roleName = await _userManager.GetRolesAsync(user);
        var role = roleName.FirstOrDefault() ?? user.Role.ToString();

        var token = GenerateJwtToken(user, role);

        return Ok(new
        {
            token,
            userId,
            role = role,
            email = user.Email
        });
    }


    //Authorization

    //Farmer
    [HttpGet("AuthorizedAdmin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AuthorizedAdmin()
    {
        return Ok("Welcome Admin!");
    }

    //Farmer
    [HttpGet("AuthorizedFarmer")]
    [Authorize(Roles = "Farmer")]
    public IActionResult AuthorizedFarmer()
    {
        return Ok("Welcome Farmer!");
    }

    [Authorize(Roles = "Farmer")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetFarmerDashboardStats()
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var totalListings = await _context.CropListings.CountAsync(c => c.FarmerId == farmerId);
        var totalTransactions = await _context.Transactions.CountAsync(t => t.FarmerId == farmerId);
        var totalNegotiations = await _context.PriceNegotiations.CountAsync(n => n.FarmerId == farmerId);


        var listingsPerMonth = await _context.CropListings
            .Where(c => c.FarmerId == farmerId)
            .GroupBy(c => c.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync();

        var cropTypeDistribution = await _context.CropListings
            .Where(c => c.FarmerId == farmerId)
            .GroupBy(c => c.Crop.Type)
            .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        return Ok(new
        {
            totalListings,
            totalTransactions,
            totalNegotiations,
            listingsPerMonth,
            cropTypeDistribution
        });
    }



    //Delaer
    [HttpGet("AuthorizedDealer")]
    [Authorize(Roles = "Dealer")]
    public IActionResult AuthorizedDealer()
    {
        return Ok("Welcome Dealer!");
    }
    [Authorize(Roles = "Dealer")]
    [HttpGet("DealerDashboard")]
    public async Task<IActionResult> GetDealerDashboardStats()
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var totalSubscriptions = await _context.Subscriptions
            .CountAsync(s => s.DealerId == dealerId);

        var totalNegotiations = await _context.PriceNegotiations
            .CountAsync(n => n.DealerId == dealerId);

        var totalTransactions = await _context.Transactions
            .CountAsync(t => t.DealerId == dealerId);

        var monthlySubscriptions = await _context.Subscriptions
            .Where(s => s.DealerId == dealerId)
            .GroupBy(s => s.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync();

        return Ok(new
        {
            totalSubscriptions,
            totalNegotiations,
            totalTransactions,
            monthlySubscriptions
        });
    }



    //Method GenerateJwtToken
    private string GenerateJwtToken(User user, string role)
    {
        var secret = _configuration["JWT:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new Exception("JWT Secret is missing in appsettings.json");
        }

        var key = Encoding.UTF8.GetBytes(secret);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwtString = tokenHandler.WriteToken(token);
        Console.WriteLine("Generated JWT: " + jwtString);

        return jwtString;
    }



    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Email not found." });

        var otp = new Random().Next(100000, 999999).ToString();
        user.PasswordResetToken = otp;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10); // Add to ApplicationUser
        await _userManager.UpdateAsync(user);

        await _emailService.SendEmailAsync(user.Email, "Password Reset OTP", $"Your OTP is: {otp}");
        return Ok(new { message = "OTP sent to email." });
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid email." });

        if (user.PasswordResetToken != request.Otp || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return BadRequest(new { message = "Invalid or expired OTP." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Clear OTP
        Console.WriteLine($"Email: {request.Email}");
        Console.WriteLine($"Otp: {request.Otp}");
        Console.WriteLine($"NewPassword: {request.NewPassword}");

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Password reset successful." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return BadRequest(new { message = "Invalid Email" });

        var isValid = await _otpService.ValidateOtpAsync(model.Email, model.Otp);
        if (!isValid) return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified successfully" });
    }


    [HttpGet("LoginWithGoogle")]
    public IActionResult LoginWithGoogle(string? returnUrl = "/", UserRole? role = null)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        var redirectUrl = Url.Action(nameof(GoogleCallback), new { returnUrl, role });
        var props = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(props, "Google");
    }

    [HttpGet("GoogleCallback")]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = "/", UserRole? role = null)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest("Google didn't send login info.");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        User user;

        if (signInResult.Succeeded)
        {
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
                return BadRequest("User not found after login.");
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Google didn't share your email.");
            }

            var newUser = new User
            {
                UserName = email,
                Email = email,
                Name = email,
                Role = role ?? UserRole.Farmer,
                Status = UserStatus.Active,
                AverageRating = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Location = "",
                PhoneNumber = ""
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(e => e.Description));
            }

            await _userManager.AddLoginAsync(newUser, info);
            await _userManager.AddClaimAsync(newUser, new Claim(ClaimTypes.Role, newUser.Role.ToString()));

            await _signInManager.SignInAsync(newUser, isPersistent: false);

            user = newUser;
        }

        // Generate JWT token
        var token = GenerateJwtToken(user, user.Role.ToString());

        // Determine frontend dashboard URL based on role
        string dashboardUrl = user.Role switch
        {
            UserRole.Farmer => "http://localhost:4200/farmerdashboard",
            UserRole.Dealer => "http://localhost:4200/dealerdashboard",
            UserRole.Admin => "http://localhost:4200/admindashboard",
            _ => "http://localhost:4200"
        };

        var finalReturnUrl = $"http://localhost:4200/auth-callback?token={token}&role={user.Role}";
        return Redirect(finalReturnUrl);
    }

    [HttpGet("debug-user")]
    [Authorize]
    public async Task<IActionResult> DebugUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            userId,
            userRole = user?.Role.ToString(),
            assignedRoles = roles,
            tokenClaims = claims
        });
    }

}

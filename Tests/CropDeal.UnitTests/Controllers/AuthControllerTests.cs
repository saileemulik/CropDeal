using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using CropDeal.Controllers;
using CropDeal.DTO;
using CropDeal.Models;
using CropDeal.Interface;
using CropDeal.UnitTests.Helpers;

namespace CropDeal.UnitTests.Controllers;


[TestFixture]
public class AuthControllerTests
{
  
    private Mock<UserManager<User>> fakeUserManager;
    private Mock<SignInManager<User>> fakeSignInManager;
    private Mock<RoleManager<IdentityRole<Guid>>> fakeRoleManager;
    private Mock<IConfiguration> fakeConfig;
    private Mock<IEmailServiceRepository> fakeEmailService;
    private Mock<IOtpService> fakeOtpService;
    private AuthController controller;

    // This runs before each test - like getting ready to play
    [SetUp]
    public void GetReady()
    {
        // Make a fake database
        var fakeDatabase = TestDbContextFactory.CreateFakeDatabase();
        
        // Make all the fake helpers
        fakeUserManager = MakeFakeUserManager();
        fakeSignInManager = MakeFakeSignInManager();
        fakeRoleManager = MakeFakeRoleManager();
        fakeConfig = new Mock<IConfiguration>();
        fakeEmailService = new Mock<IEmailServiceRepository>();
        fakeOtpService = new Mock<IOtpService>();

        // Tell the fake config what to say
        fakeConfig.Setup(c => c["JWT:Secret"]).Returns("my-super-secret-key-for-testing-that-is-long-enough-32-chars");
        fakeConfig.Setup(c => c["JWT:ValidIssuer"]).Returns("test-app");
        fakeConfig.Setup(c => c["JWT:ValidAudience"]).Returns("test-users");

        // Make the controller with all fake stuff
        controller = new AuthController(
            fakeUserManager.Object,
            fakeDatabase,
            fakeOtpService.Object,
            fakeEmailService.Object,
            fakeSignInManager.Object,
            fakeRoleManager.Object,
            fakeConfig.Object
        );
    }

    // Test: Can someone sign up with good information?
    [Test]
    public async Task CanSignUpWithGoodInfo()
    {
        // Make some good sign up information
        var signUpInfo = new SignUpDto
        {
            Name = "John Doe",
            Email = "john@test.com",
            Password = "GoodPassword123!",
            PhoneNumber = "9908212345",
            Location = "Test City"
        };

        // Tell our fake helpers to say "yes, everything is good"
        fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        fakeRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        fakeUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Try to sign up
        var result = await controller.SignUp(signUpInfo, UserRole.Farmer);

        // Check: Did it work? (Should say "OK")
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test: Can someone login with correct password?
    [Test]
    public async Task CanLoginWithCorrectPassword()
    {
        // Make login information
        var loginInfo = new SignInDto { Email = "john@test.com", Password = "CorrectPassword" };
        
        // Make a fake user
        var fakeUser = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "john@test.com", 
            UserName = "john@test.com", 
            Role = UserRole.Farmer,
            Status = UserStatus.Active
        };

        // Tell fake helpers what to do
        fakeUserManager.Setup(x => x.FindByEmailAsync(loginInfo.Email)).ReturnsAsync(fakeUser);
        fakeUserManager.Setup(x => x.GetUserIdAsync(fakeUser)).ReturnsAsync(fakeUser.Id.ToString());
        fakeUserManager.Setup(x => x.IsInRoleAsync(fakeUser, "Admin")).ReturnsAsync(false);
        fakeSignInManager.Setup(x => x.PasswordSignInAsync(fakeUser, loginInfo.Password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        fakeUserManager.Setup(x => x.GetRolesAsync(fakeUser)).ReturnsAsync(new List<string> { "Farmer" });

        // Try to login
        var result = await controller.SignIn(loginInfo);

        // Check: Did it work? (Should say "OK")
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    // Test: What happens with wrong password?
    [Test]
    public async Task CantLoginWithWrongPassword()
    {
        // Try to login with wrong info
        var badLoginInfo = new SignInDto { Email = "john@test.com", Password = "WrongPassword" };

        // Tell fake helper: "No user found with this email"
        fakeUserManager.Setup(x => x.FindByEmailAsync(badLoginInfo.Email)).ReturnsAsync((User?)null);

        // Try to login
        var result = await controller.SignIn(badLoginInfo);

        // Check: Did it fail? (Should say "Not allowed")
        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    // Helper: Make a fake user manager
    private Mock<UserManager<User>> MakeFakeUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    // Helper: Make a fake sign in manager
    private Mock<SignInManager<User>> MakeFakeSignInManager()
    {
        return new Mock<SignInManager<User>>(
            fakeUserManager.Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null, null, null, null);
    }

    // Helper: Make a fake role manager
    private Mock<RoleManager<IdentityRole<Guid>>> MakeFakeRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole<Guid>>>();
        return new Mock<RoleManager<IdentityRole<Guid>>>(store.Object, null, null, null, null);
    }
}
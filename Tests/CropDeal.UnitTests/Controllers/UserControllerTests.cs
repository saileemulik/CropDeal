using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.DTO;
using CropDeal.Models;
using CropDeal.Interface;

namespace CropDeal.UnitTests.Controllers;

// Tests for the User Profile controller
[TestFixture]
public class UserControllerTests
{
    // These are fake helpers
    private Mock<IUserRepository> fakeUserRepository;
    private Mock<UserManager<User>> fakeUserManager;
    private UserProfileController controller;
    private ClaimsPrincipal fakeLoggedInUser;

    // This runs before each test - like getting ready to play
    [SetUp]
    public void GetReady()
    {
        // Make fake helpers
        fakeUserRepository = new Mock<IUserRepository>();
        fakeUserManager = MakeFakeUserManager();

        // Make the controller with fake stuff
        controller = new UserProfileController(fakeUserRepository.Object, fakeUserManager.Object);

        // Pretend someone is logged in
        var userId = Guid.NewGuid().ToString();
        fakeLoggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        // Tell the controller who is "logged in"
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = fakeLoggedInUser }
        };
    }

    // Test: Can someone see their profile?
    [Test]
    public async Task CanSeeMyProfile()
    {
        // Get the fake user's ID
        var userId = fakeLoggedInUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Make some fake user information
        var fakeUserInfo = new UserDto
        {
            Id = Guid.Parse(userId!),
            Name = "John Doe",
            Email = "john@test.com",
            PhoneNumber = "1234567890",
            Location = "Test City"
        };

        // Tell fake repository: "When asked for user, give this fake info"
        fakeUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(fakeUserInfo);

        // Try to get profile
        var result = await controller.GetMyProfile();

        // Check: Did it work? (Should say "OK" and give back the info)
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(fakeUserInfo));
    }

    // Test: What happens if user doesn't exist?
    [Test]
    public async Task CantSeeProfileIfUserDoesntExist()
    {
        // Tell fake repository: "No user found"
        fakeUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDto?)null);

        // Try to get profile
        var result = await controller.GetMyProfile();

        // Check: Did it fail? (Should say "Not found")
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    // Test: Can someone update their profile?
    [Test]
    public async Task CanUpdateMyProfile()
    {
        // Get the fake user's ID
        var userId = fakeLoggedInUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Make new information to update
        var newInfo = new UpdateUserDto
        {
            Name = "John Smith",
            Email = "johnsmith@test.com",
            PhoneNumber = "9876543210",
            Location = "New City"
        };

        // Make a fake existing user
        var existingUser = new User
        {
            Id = Guid.Parse(userId!),
            Name = "Old Name",
            Email = "old@test.com"
        };

        // Tell fake repository what to do
        fakeUserRepository.Setup(x => x.GetUserByIdAsModelAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);
        fakeUserRepository.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Try to update profile
        var result = await controller.UpdateMyProfile(newInfo);

        // Check: Did it work? (Should say "OK")
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        // Check: Did it actually try to save? (Should call update once)
        fakeUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    // Test: What happens if trying to update non-existent user?
    [Test]
    public async Task CantUpdateIfUserDoesntExist()
    {
        // Make update information
        var newInfo = new UpdateUserDto
        {
            Name = "New Name",
            Email = "new@test.com"
        };

        // Tell fake repository: "No user found"
        fakeUserRepository.Setup(x => x.GetUserByIdAsModelAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        // Try to update
        var result = await controller.UpdateMyProfile(newInfo);

        // Check: Did it fail? (Should say "Not found")
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // Test: What happens if nobody is logged in?
    [Test]
    public void CantSeeProfileIfNotLoggedIn()
    {
        // Pretend nobody is logged in
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Try to get profile
        var result = controller.GetMyProfile().Result;

        // Check: Did it fail? (Should say "Not allowed")
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    // Helper: Make a fake user manager
    private Mock<UserManager<User>> MakeFakeUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }
}
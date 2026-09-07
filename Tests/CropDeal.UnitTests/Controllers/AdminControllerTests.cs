using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CropDeal.Controllers;
using CropDeal.Interface;
using CropDeal.Models;
using CropDeal.AppDB;
using CropDeal.UnitTests.Helpers;

namespace CropDeal.UnitTests.Controllers
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminRepository> mockAdminRepo;
        private Mock<ICropListingRepository> mockCropRepo;
        private Mock<IEmailServiceRepository> mockEmailService;
        private AdminController adminController;
        private CropDealDBContext dbContext;

        [SetUp]
        public void SetUp()
        {
            mockAdminRepo = new Mock<IAdminRepository>();
            mockCropRepo = new Mock<ICropListingRepository>();
            mockEmailService = new Mock<IEmailServiceRepository>();
            dbContext = TestDbContextFactory.CreateFakeDatabase();
            adminController = new AdminController(mockAdminRepo.Object, dbContext, mockCropRepo.Object, mockEmailService.Object);
        }

        [Test]
        public async Task GetAllUsers_ReturnsOkResult_WhenUsersExist()
        {
            var testUsers = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Name = "Test User 1" },
                new UserDto { Id = Guid.NewGuid(), Name = "Test User 2" }
            };
            mockAdminRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(testUsers);

            var result = await adminController.GetAllUsers();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }
    }
}
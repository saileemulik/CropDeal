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
    public class CropListingControllerTests
    {
        private Mock<ICropListingRepository> mockCropListingRepo;
        private Mock<IEmailServiceRepository> mockEmailService;
        private CropListingController cropListingController;
        private CropDealDBContext dbContext;

        [SetUp]
        public void SetUp()
        {
            mockCropListingRepo = new Mock<ICropListingRepository>();
            mockEmailService = new Mock<IEmailServiceRepository>();
            dbContext = TestDbContextFactory.CreateFakeDatabase();
            cropListingController = new CropListingController(mockCropListingRepo.Object, dbContext, mockEmailService.Object);
        }

        // [Test]
        // public async Task GetAllCropListings_ReturnsOkResult_WhenListingsExist()
        // {
        //     var testListings = new List<CropListing>
        //     {
        //         new CropListing { Id = Guid.NewGuid(), Description = "Test Listing 1" },
        //         new CropListing { Id = Guid.NewGuid(), Description = "Test Listing 2" }
        //     };
        //     mockCropListingRepo.Setup(r => r.GetAllListingsForAdminAsync()).ReturnsAsync(testListings);

        //     var result = await cropListingController.GetAllCropListings();

        //     Assert.That(result, Is.InstanceOf<OkObjectResult>());
        // }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }
    }
}
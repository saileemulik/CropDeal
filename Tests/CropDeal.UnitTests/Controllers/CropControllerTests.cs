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
    public class CropControllerTests
    {
        private Mock<ICropRepository> mockCropRepo;
        private Mock<IEmailServiceRepository> mockEmailService;
        private CropController cropController;
        private CropDealDBContext dbContext;

        [SetUp]
        public void SetUp()
        {
            mockCropRepo = new Mock<ICropRepository>();
            mockEmailService = new Mock<IEmailServiceRepository>();
            dbContext = TestDbContextFactory.CreateFakeDatabase();
            cropController = new CropController(mockCropRepo.Object, dbContext, mockEmailService.Object);
        }

        [Test]
        public async Task GetAllCrops_ReturnsOkResult_WhenCropsExist()
        {
            var testCrops = new List<Crop>
            {
                new Crop { Id = Guid.NewGuid(), Name = "Test Crop 1" },
                new Crop { Id = Guid.NewGuid(), Name = "Test Crop 2" }
            };
            mockCropRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(testCrops);

            var result = await cropController.GetAllCrops();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }
    }
}
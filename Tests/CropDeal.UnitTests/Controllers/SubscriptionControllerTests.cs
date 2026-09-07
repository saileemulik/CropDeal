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
    public class SubscriptionControllerTests
    {
        private Mock<ISubscriptionRepository> mockSubscriptionRepo;
        private SubscriptionController subscriptionController;
        private CropDealDBContext dbContext;

        [SetUp]
        public void SetUp()
        {
            mockSubscriptionRepo = new Mock<ISubscriptionRepository>();
            dbContext = TestDbContextFactory.CreateFakeDatabase();
            subscriptionController = new SubscriptionController(mockSubscriptionRepo.Object, dbContext);
        }

        // [Test]
        // public async Task GetMySubscriptions_ReturnsResult_WhenCalled()
        // {
        //     var testSubscriptions = new List<Subscription>
        //     {
        //         new Subscription { Id = Guid.NewGuid(), CropName = "Wheat" },
        //         new Subscription { Id = Guid.NewGuid(), CropName = "Rice" }
        //     };
        //     mockSubscriptionRepo.Setup(r => r.GetSubscriptionsByDealerIdAsync(It.IsAny<Guid>())).ReturnsAsync(testSubscriptions);

        //     var result = await subscriptionController.GetMySubscriptions();

        //     Assert.That(result, Is.InstanceOf<IActionResult>());
        // }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }
    }
}
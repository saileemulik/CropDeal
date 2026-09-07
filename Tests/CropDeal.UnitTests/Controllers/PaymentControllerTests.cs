using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CropDeal.Controllers;
using CropDeal.Interface;
using CropDeal.Models;
using CropDeal.UnitTests.Helpers;

namespace CropDeal.UnitTests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentRepository> mockPaymentRepo;
        private Mock<ITransactionRepository> mockTransactionRepo;
        private Mock<INegotiationRepository> mockNegotiationRepo;
        private PaymentController paymentController;

        [SetUp]
        public void SetUp()
        {
            mockPaymentRepo = new Mock<IPaymentRepository>();
            mockTransactionRepo = new Mock<ITransactionRepository>();
            mockNegotiationRepo = new Mock<INegotiationRepository>();
            paymentController = new PaymentController(mockPaymentRepo.Object, mockTransactionRepo.Object, mockNegotiationRepo.Object);
        }

        [Test]
        public async Task CreateOrder_ReturnsResult_WhenCalled()
        {
            var request = new CreatePaymentOrderRequestDto { TotalPrice = 100.0f, Currency = "INR" };

            var result = await paymentController.CreateOrder(request);

            Assert.That(result, Is.InstanceOf<IActionResult>());
        }

        [TearDown]
        public void TearDown()
        {
            // Controllers don't implement IDisposable
        }
    }
}
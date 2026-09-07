namespace CropDeal.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepo;
     private readonly ITransactionRepository _transRepo;
     private readonly INegotiationRepository _negotiationRepo;
    private const string RazorpayKey = "rzp_test_flDOA2YudIMyRX";
    private const string RazorpaySecret = "iZunrz6EkebQT914Y3LBvXp9";

    public PaymentController(IPaymentRepository paymentRepo, ITransactionRepository transRepo, INegotiationRepository negotiationRepo)
    {
        _paymentRepo = paymentRepo;
        _transRepo = transRepo;
        _negotiationRepo = negotiationRepo;
    }

     [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentOrderRequestDto paymentRequest)
    {
        if (paymentRequest == null || paymentRequest.TotalPrice <= 0)
        {
            return BadRequest("Invalid request parameters.");
        }


        var razorpayOrder = await CreateRazorpayOrder(paymentRequest.TotalPrice, paymentRequest.Currency);

        if (razorpayOrder == null)
        {
            return BadRequest("Failed to create Razorpay order.");
        }

       return Ok(new
{
    amount = razorpayOrder["amount"],        // amount in paisa
    currency = razorpayOrder["currency"].ToString(),
    orderId = razorpayOrder["id"].ToString()
});
    }
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyPaymentRequestDto request)
    {
        var paymentVerified = await _paymentRepo.VerifyPaymentAsync(request);

    if (!paymentVerified)
        return BadRequest(new { success = false, message = "Payment verification failed." });

    var updated = await _transRepo.VerifyTransactionAndUpdateListingAsync(request.OrderId);

    if (!updated)
        return BadRequest(new { success = false, message = "Transaction update failed." });

    return Ok(new { success = true, message = "Payment verified and transaction updated." });
    }

    private async Task<Order> CreateRazorpayOrder(float totalPrice, string currency)
    {
        try
        {
            var client = new RazorpayClient(RazorpayKey, RazorpaySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", totalPrice * 100 },
                { "currency",currency },
                { "receipt", "rcptid_11" },
                { "payment_capture", 1 }
            };

            var order = client.Order.Create(options);
            return order;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine("Error creating Razorpay order: " + ex.Message);
            return null;
        }
    }
}

namespace CropDeal.Repository;

public class PaymentRepository : IPaymentRepository
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly RazorpayClient _razorpayClient;
    private readonly IConfiguration _config;
    private readonly INegotiationRepository _negotiationRepo;

    public PaymentRepository(ITransactionRepository transactionRepo,INegotiationRepository negotiationRepo, IConfiguration config)
    {
        _transactionRepo = transactionRepo;
        _config = config;
         _negotiationRepo = negotiationRepo;
        _razorpayClient = new RazorpayClient(
            config["Razorpay:Key"], config["Razorpay:Secret"]);
    }

    public async Task<string> CreateOrderAsync(Guid transactionId)
    {
        var transaction = await _transactionRepo.GetByIdAsync(transactionId);
       var negotiation = await _negotiationRepo.GetNegotiationByDealerAndListingAsync(transaction.DealerId, transaction.ListingId);
    if (negotiation != null && negotiation.NegotiatedPricePerKg > 0)
    {
        transaction.TotalPrice = negotiation.NegotiatedPricePerKg;
        await _transactionRepo.UpdateAsync(transaction); // Save updated price
    }
        var options = new Dictionary<string, object>
    {
        { "amount", (int)(transaction.TotalPrice * 100) },
        { "currency", "INR" },
        { "receipt", transaction.Id.ToString() }
    };

        var order = _razorpayClient.Order.Create(options);
        Console.WriteLine("Razorpay Order Response: " + order);

        // Ensure order is not null before accessing properties
        if (order == null || string.IsNullOrEmpty(order["id"].ToString()))
        {
            Console.WriteLine("Error: Razorpay failed to generate OrderId.");
            return null;
        }

        transaction.OrderId = order["id"].ToString();
        await _transactionRepo.UpdateAsync(transaction);

        return transaction.OrderId;
    }


    public async Task<bool> VerifyPaymentAsync(VerifyPaymentRequestDto request)
    {
        var generatedSignature = GetSignature(request.OrderId + "|" + request.PaymentId);
        if (generatedSignature == request.Signature)
        {
            var transaction = await _transactionRepo.GetByIdAsync(request.Id);
            transaction.PaymentId = request.PaymentId;
            transaction.Status = TransactionStatus.Completed;
            transaction.PaidAt = DateTime.UtcNow;
            await _transactionRepo.UpdateAsync(transaction);
            return true;
        }
        return false;
    }

    private string GetSignature(string payload)
{
    var secret = Encoding.UTF8.GetBytes(_config["Razorpay:Secret"]);
    using var hmac = new HMACSHA256(secret);
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Razorpay expects lowercase hex
}
}

using System.Text.Json;

namespace CropDeal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionRepository _transRepo;
    private readonly ICropListingRepository _cropRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IInvoiceServiceRepository _invoiceService;

    private readonly INegotiationRepository _negotiationRepo;

    public TransactionController(ITransactionRepository transRepo, ILogger<TransactionController> logger, INegotiationRepository negotiationRepo, ICropListingRepository cropRepo, IPaymentRepository paymentRepo, IInvoiceServiceRepository invoiceService)
    {
        _transRepo = transRepo;
        _cropRepo = cropRepo;
        _paymentRepo = paymentRepo;
        _invoiceService = invoiceService;
        _logger = logger;
        _negotiationRepo = negotiationRepo;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Admin/GetAllTransactions")]
    public async Task<IActionResult> GetAll()
    {
        var allTxns = await _transRepo.GetAllTransactionsAsync();
        return Ok(new { allTxns });
    }


    [HttpGet("Dealer/GetMyTransactions")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> GetMyTransactionsForDealer()
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var txns = await _transRepo.GetTransactionsByDealerIdAsync(dealerId);
        return Ok(new { txns });
    }

    [HttpGet("Farmer/GetMyTransactions")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> GetMyTransactionsForFarmer()
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var txns = await _transRepo.GetTransactionsByFarmerIdAsync(farmerId);
        return Ok(new { txns });
    }

    [HttpPost("Dealer/CreateTransaction")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
    {
        try
        {
            Console.WriteLine("Received Request: " + JsonSerializer.Serialize(dto));

            if (string.IsNullOrEmpty(dto.PaymentGateway))
                return BadRequest(new { message = "PaymentGateway is required." });

            var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listing = await _cropRepo.GetCropByIdAsync(dto.ListingId);
            if (listing == null)
                return NotFound(new { message = "Crop listing not found." });

            if (dto.Quantity <= 0 || dto.Quantity > listing.Quantity)
                return BadRequest(new { message = "Invalid quantity or insufficient stock." });

            var totalPrice = await CalculateTotalPriceAsync(dealerId, dto.ListingId, dto.Quantity);
            if (dto.TotalPrice <= 0)
                return BadRequest(new { message = "Total price must be greater than zero." });

            if (dto.TotalPrice < listing.PricePerKg * dto.Quantity * 0.5) // optional sanity check
                return BadRequest(new { message = "Total price seems too low. Please enter a valid agreed price." });


            // Step 1: Create transaction without OrderId
            var transaction = new Transaction
            {
                DealerId = dealerId,
                ListingId = dto.ListingId,
                Quantity = dto.Quantity,
                TotalPrice = totalPrice,
                OrderId = dto.OrderId, // null initially
                Currency = dto.Currency,
                PaymentGateway = dto.PaymentGateway,
                Status = TransactionStatus.Pending,
                PaidAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PaymentId = null,
            };

            var created = await _transRepo.AddTransactionAsync(transaction);

            if (created == null)
                return StatusCode(500, new { message = "Failed to create transaction. Please try again later." });

            // Step 2: Generate payment order using transaction ID
            var generatedOrderId = await _paymentRepo.CreateOrderAsync(created.Id);

            if (string.IsNullOrEmpty(generatedOrderId))
            {
                // Optional: rollback or mark transaction as failed
                return StatusCode(500, new { message = "Failed to create payment order." });
            }

            // Step 3: Update transaction with OrderId
            created.OrderId = generatedOrderId;
            var updated = await _transRepo.UpdateOrderIdAsync(created.Id, generatedOrderId);

            if (!updated)
            {
                Console.WriteLine($"Warning: Failed to update transaction {created.Id} with OrderId {generatedOrderId}");
                // You might want to handle this case, e.g. retry or notify admins
            }

            return Ok(new
            {
                transaction = created,
                orderId = generatedOrderId,
                message = "Transaction created and payment order generated successfully."
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR in CreateTransaction: " + ex.Message);
            return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
        }
    }



    [HttpPost("Dealer/PayTransaction/{id}")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> PayTransaction(Guid id)
    {
        var dealerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var txns = await _transRepo.GetTransactionsByDealerIdAsync(dealerId);

        var txn = txns.FirstOrDefault(t => t.Id == id);
        if (txn == null)
            return NotFound(new { message = "Transaction not found." });

        if (txn.Status == TransactionStatus.Completed)
            return BadRequest(new { message = "Transaction already completed." });

        var success = await _transRepo.UpdateTransactionStatusAsync(id, TransactionStatus.Completed);
        if (!success)
            return BadRequest(new { message = "Payment failed or transaction update error." });

        return Ok(new { message = "Payment successful. Transaction marked as completed." });
    }

    [HttpPut("{id}/set-order-id")]
    [Authorize(Roles = "Dealer")]
    public async Task<IActionResult> SetOrderId(Guid id, [FromBody] UpdateOrderIdDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _transRepo.UpdateOrderIdAsync(id, dto.OrderId);
        if (!result)
            return NotFound("Transaction not found");

        return Ok(new { success = true, message = "Order ID updated successfully" });
    }


    [HttpGet("DownloadInvoice/{transactionId}")]
    [Authorize(Roles = "Dealer, Farmer")]
    public async Task<IActionResult> DownloadInvoice(Guid transactionId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var txn = await _transRepo.GetByIdAsync(transactionId);
            if (txn == null)
            {
                return NotFound(new { message = "Transaction not found." });
            }

            // Validate based on role
            if ((userRole == "Dealer" && txn.DealerId != userId) ||
                (userRole == "Farmer" && txn.FarmerId != userId))
            {
                return Unauthorized(new { message = "You are not authorized to access this invoice." });
            }

            var pdfStream = _invoiceService.GenerateInvoicePdf(txn);

            if (pdfStream == null || pdfStream.Length == 0)
            {
                _logger.LogError("Invoice service returned null or empty stream.");
                return StatusCode(500, "Failed to generate invoice.");
            }

            return File(pdfStream, "application/pdf", $"Invoice_{txn.Id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invoice generation failed for transaction {TransactionId}", transactionId);
            return StatusCode(500, "Internal server error while generating invoice.");
        }
    }

    [HttpPut("Admin/{id}/Status")]
    public async Task<IActionResult> UpdateTransactionStatus(Guid id, [FromBody] TransactionStatus status)
    {
        var result = await _transRepo.UpdateTransactionStatusAsync(id, status);
        if (!result)
        {
            return NotFound(new { Message = "Transaction not found" });
        }

        return Ok(new { Message = "Transaction status updated successfully" });
    }
    private async Task<float> CalculateTotalPriceAsync(Guid dealerId, Guid listingId, int quantity)
    {
        var listing = await _cropRepo.GetCropByIdAsync(listingId);
        if (listing == null)
            throw new Exception("Listing not found");

        var negotiation = await _negotiationRepo.GetNegotiationByDealerAndListingAsync(dealerId, listingId);
        float pricePerKg = listing.PricePerKg;

        if (negotiation != null && negotiation.Status == PriceStatus.Accepted)
        {
            quantity = negotiation.Quantity;
            pricePerKg = negotiation.NegotiatedPricePerKg;
        }

        var totalPrice = pricePerKg * quantity;

        if (totalPrice <= 0)
            throw new Exception("Total price must be greater than zero.");

        // Assuming you want to check if totalPrice is at least 50% of normal price * quantity
        if (totalPrice < listing.PricePerKg * quantity * 0.5f)
            throw new Exception("Total price too low.");

        return totalPrice;
    }

[HttpGet("Dealer/GetTransactionByDealerAndListing/{dealerId}/{listingId}")]
    public async Task<IActionResult> GetTransactionByDealerAndListing(Guid dealerId, Guid listingId)
    {
        var txn = await _transRepo.GetByDealerAndListingAsync(dealerId, listingId);
        if (txn == null) return NotFound();

        return Ok(txn);
    }


}

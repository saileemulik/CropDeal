namespace CropDeal.DTO;

public class CreateTransactionDto
{
    [Required]
    public Guid ListingId { get; set; }

    // [Required]
    // public Guid DealerId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public float TotalPrice { get; set; }

    // [Required]
    public string? OrderId { get; set; }

    [Required]
    public string Currency { get; set; }

    // [Required]
    public string? PaymentId { get; set; }

    [Required]
    public string PaymentGateway { get; set; }
}

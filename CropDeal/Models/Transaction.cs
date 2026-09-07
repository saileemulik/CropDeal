namespace CropDeal.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Dealer")]
    public Guid DealerId { get; set; }

    [ForeignKey("Farmer")]
    public Guid FarmerId { get; set; }


    [ForeignKey("CropListing")]
    public Guid ListingId { get; set; }


    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }


    [Required]
    [Range(0.01, float.MaxValue, ErrorMessage = "Total Price must be greater than 0")]
    public float TotalPrice { get; set; }


    [Required]
    public TransactionStatus Status { get; set; }

    [Required]
    public string PaymentGateway { get; set; }
    
    // [Required]
    public string? PaymentId { get; set; }

    // [Required]
    public string? OrderId { get; set; }

    [Required]
    public string Currency { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

  
    [JsonIgnore]
    public User? Dealer { get; set; }

    [JsonIgnore]
    public User? Farmer { get; set; }


    [JsonIgnore]
    public CropListing? Listing { get; set; }
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled
}

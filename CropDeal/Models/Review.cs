namespace CropDeal.Models;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Dealer")]
    public Guid DealerId { get; set; }

    [ForeignKey("Farmer")]
    public Guid FarmerId { get; set; }

    [ForeignKey("Transaction")]
    public Guid TransactionId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public float Rating { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    public string Comment { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }


    [JsonIgnore]
    public User? Dealer { get; set; }

    [JsonIgnore]
    public User? Farmer { get; set; }

    [JsonIgnore]
    public Transaction? Transaction { get; set; }
}

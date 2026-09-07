namespace CropDeal.Models;

public class Subscription
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Dealer")]
    public Guid DealerId { get; set; }

    [ForeignKey("CropListing")]
    public Guid? CropListingId { get; set; }
    public string? CropName { get; set; }

    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public User Dealer { get; set; }

    [JsonIgnore]
    public CropListing CropListing { get; set; }
}

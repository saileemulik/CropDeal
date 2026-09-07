namespace CropDeal.Models;

public class PickupSchedule
{
    public Guid Id { get; set; }

    [ForeignKey("CropListing")]
    public Guid ListingId { get; set; }

    [ForeignKey("Farmer")]
    public Guid FarmerId { get; set; }
    [ForeignKey("Dealer")]
    public Guid DealerId { get; set; }

    public DateTime ProposedPickupSlot { get; set; }
    public DateTime? ConfirmedPickupSlot { get; set; }

    public PickupStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public CropListing? CropListing { get; set; }

    [JsonIgnore]
    public User? Farmer { get; set; }
    
    [JsonIgnore]
    public User? Dealer { get; set; }
}
public enum PickupStatus
{
    Proposed,
    Confirmed,
    Scheduled,
    InTransit,
    Delivered,
    Cancelled
}


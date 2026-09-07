namespace CropDeal.DTO;

public class PriceNegotiationRequestDto
{
    [Required]
    public Guid ListingId { get; set; }
    [Required]
    public Guid DealerId { get; set; }
    [Required]
    public Guid FarmerId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public float NegotiatedPricePerKg { get; set; }
    [Required]
    public float TotalPrice { get; set; }
    public PriceStatus Status { get; set; } = PriceStatus.Pending;
}

namespace CropDeal.DTO;

public class CropListingDto
{
    public Guid Id { get; set; }
    public Guid CropId { get; set; }
    public Guid FarmerId { get; set; }
    public string Description { get; set; }
    public double Quantity { get; set; }
    public float PricePerKg { get; set; }
    public string Unit { get; set; }
    public CropAvailability Status { get; set; }
     public string ImageBase64 { get; set; } 
    public string Location { get; set; }
    public string LocationCategory { get; set; }
}

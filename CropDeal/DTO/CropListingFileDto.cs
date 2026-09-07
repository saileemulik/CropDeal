namespace CropDeal.DTO;

public class CropListingFileDto
{
    public Guid CropId { get; set; }
    public float PricePerKg { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }
    public string Location { get; set; }
    public CropAvailability Status { get; set; }
    public string Description { get; set; }
}
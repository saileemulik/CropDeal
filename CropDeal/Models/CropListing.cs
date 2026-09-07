namespace CropDeal.Models;

public class CropListing
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Farmer")]
    public Guid FarmerId { get; set; }

    [ForeignKey("Crop")]
    public Guid CropId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Price per Kg must be a positive value")]
    public float PricePerKg { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [MaxLength(10, ErrorMessage = "Unit should be concise (e.g., 'Kg')")]
    public string Unit { get; set; }


    [Required]
    // [SwaggerSchema("City where the crop is located. Only alphabets and spaces are allowed.")]
    [MaxLength(100, ErrorMessage = "Location should not exceed 100 characters")]
    public string Location { get; set; }


    [Required]
    public CropAvailability Status { get; set; }



    public byte[]? ImageUrl { get; set; }


    [NotMapped]
    public string? ImageBase64 { get; set; }

    [Required]
    [MaxLength(500, ErrorMessage = "Description can be up to 500 characters")]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public User? Farmer { get; set; }

    [JsonIgnore]
    public Crop? Crop { get; set; }

    // [JsonIgnore]
    // public Address? Address { get; set; }
    
    [JsonIgnore]
    public ICollection<PickupSchedule>? PickupSchedules { get; set; }


}
public enum CropAvailability
{
    Available,
    OutOfStock
}

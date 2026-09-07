namespace CropDeal.Models;

public class Crop
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [ForeignKey("Admin")]
    public Guid AdminId { get; set; }


    public CropTypeEnum Type { get; set; }
    public string? CustomType { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public User? Admin { get; set; }
}

public enum CropTypeEnum
{
    Fruit,
    Vegetable,
    Grain
}



namespace CropDeal.Models;
public class Address
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[A-Za-z0-9,.-]{3,100}$",ErrorMessage ="Invalid street name")]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Za-z]{3,50}$", ErrorMessage ="Invalid city name")]
    public string City { get; set; }

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Za-z]{3,50}$", ErrorMessage ="Invalid state name")]
    public string State { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression(@"^[0-9]{6}$", ErrorMessage ="Invalid zip code")]
    public string ZipCode { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}


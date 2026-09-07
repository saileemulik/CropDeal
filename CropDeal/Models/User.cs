namespace CropDeal.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    [Required]
    [RegularExpression(@"[A-Za-z0-9.-_%]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")]
    public override string Email { get; set; }

    [Phone(ErrorMessage = "Invalid Phone Number.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Indian mobile number")]
    [Required]
    public override string? PhoneNumber { get; set; }

    [Required]
    public UserRole Role { get; set; }

    [Required]
    public UserStatus Status { get; set; }

    // [Required]
    // public Guid? BankAccountId { get; set; }

    [Required]
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public float AverageRating { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Location should be City name and should not exceed 100 characters")]
    [RegularExpression(@"^[A-Za-z]{3,50}$", ErrorMessage ="Invalid city name")]
    public string? Location { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // [Required]
    // [JsonIgnore]
    // public BankAccount? BankAccount { get; set; }
    
    [JsonIgnore]

    public ICollection<PriceNegotiationRequest>? PriceNegotiations { get; set; }

    public ICollection<Crop>? Crops { get; set; }
    public ICollection<Subscription>? Subscriptions { get; set; }

}

public enum UserRole
{
    Farmer,
    Dealer,
    Admin
}

public enum UserStatus
{
    Active,
    Inactive
}


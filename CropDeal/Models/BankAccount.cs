namespace CropDeal.Models;
public class BankAccount
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [RegularExpression("^[0-9]{9,18}$", ErrorMessage = "Invalid account number format")]
    public string AccountNumber { get; set; }

    [Required]
    [MaxLength(11)]
    [RegularExpression("^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code format")]
    public string IFSCCode { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression("^[A-Za-z ]{3,}$", ErrorMessage = "Invalid bank name format")]
    public string BankName { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression("^[A-Za-z ]{3,}$", ErrorMessage = "Invalid branch name format")]
    public string BranchName { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }

    [ValidateNever]
    [JsonIgnore]
    public User User { get; set; }


}

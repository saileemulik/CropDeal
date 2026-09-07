namespace CropDeal.Models;
public class Report
{

    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [ForeignKey("Admin")]
    public Guid GeneratedBy { get; set; }

    [Required]
    public Guid GeneratedFor { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public User? Admin { get; set; }
}

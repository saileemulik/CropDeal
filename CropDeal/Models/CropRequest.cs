namespace CropDeal.Models;

public class CropRequest
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string CropName { get; set; }

    public string? Description { get; set; }
    public string? Type { get; set; }

    [ForeignKey("Farmer")]
    public Guid FarmerId { get; set; }

    public RequestStatus Status { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User? Farmer { get; set; }


}
public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}

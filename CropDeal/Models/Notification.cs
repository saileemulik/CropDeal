namespace CropDeal.Models;

public class Notification
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}

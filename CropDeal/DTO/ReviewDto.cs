namespace CropDeal.DTO;

public class ReviewDto
{
    public Guid FarmerId { get; set; }
    public Guid TransactionId { get; set; }
    public float Rating { get; set; }
    public string Comment { get; set; }
}

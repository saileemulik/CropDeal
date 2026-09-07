namespace CropDeal.DTO;
public class CreatePaymentOrderRequestDto
{
     public Guid Id { get; set; }
    public Guid TransactionId { get; set; } 
    
   
     public Guid ListingId { get; set; }
    public int Quantity { get; set; }
    public float TotalPrice { get; set; }
    public string Currency { get; set; }
}

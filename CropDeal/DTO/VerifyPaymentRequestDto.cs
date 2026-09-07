namespace CropDeal.DTO;

public class VerifyPaymentRequestDto
{
     public Guid Id { get; set; } // your internal transaction ID
    public string PaymentId { get; set; } // Razorpay payment ID (string!)
    public string OrderId { get; set; }
    public string Signature { get; set; }
}

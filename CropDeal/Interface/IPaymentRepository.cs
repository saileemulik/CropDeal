namespace CropDeal.Interface;

public interface IPaymentRepository
{
    Task<string> CreateOrderAsync(Guid transactionId);
    Task<bool> VerifyPaymentAsync(VerifyPaymentRequestDto request);
}

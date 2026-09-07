namespace CropDeal.Interface;

public interface IOtpService
{
    Task<bool> ValidateOtpAsync(string email, string otp);
    Task StoreOtpAsync(string email, string otp, TimeSpan duration);
}

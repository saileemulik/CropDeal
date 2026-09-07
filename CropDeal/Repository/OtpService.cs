namespace CropDeal.Repository;

public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task StoreOtpAsync(string email, string otp, TimeSpan duration)
    {
        _cache.Set(email, otp, duration);
        return Task.CompletedTask;
    }

    public Task<bool> ValidateOtpAsync(string email, string otp)
    {
        if (_cache.TryGetValue(email, out string storedOtp) && storedOtp == otp)
        {
            _cache.Remove(email);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

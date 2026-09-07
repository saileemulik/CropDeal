namespace CropDeal.Interface;

public interface ISubscriptionRepository
{
    Task<Subscription> AddSubscriptionAsync(Guid dealerId, Guid cropListingId);
    Task<IEnumerable<Subscription>> GetSubscriptionsByDealerIdAsync(Guid dealerId);
    Task<bool> DeleteSubscriptionAsync(Guid dealerId, Guid cropListingId);
    Task<IEnumerable<SubscriptionDto>> GetSubscribedListingsForFarmerAsync(Guid farmerId);
    Task<Subscription> AddCropNameSubscriptionAsync(Guid dealerId, string cropName);
    Task<bool> DeleteCropNameSubscriptionAsync(Guid dealerId, string cropName);
    Task<IEnumerable<string>> GetCropNameSubscriptionsByDealerAsync(Guid dealerId);





}

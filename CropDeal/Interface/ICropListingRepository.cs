namespace CropDeal.Interface;

public interface ICropListingRepository
{
    Task<IEnumerable<CropListingDto>> GetAllCropsForDealerAsync(Guid dealerId);
    Task<CropListing> GetCropByIdAsync(Guid id);
    Task<CropListing> AddCropAsync(Guid farmerId,CropListing listing);
    Task UpdateCropAsync(CropListing crop);
    Task DeleteCropAsync(Guid id);
    Task<IEnumerable<CropListing>> GetAllListingsForAdminAsync();
    IEnumerable<CropListing> GetListingsByUserId(Guid userId);
}

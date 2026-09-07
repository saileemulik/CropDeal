namespace CropDeal.Interface;

public interface IAddressRepository
{
    Task<Address> AddAddressAsync(Address address);
    Task<Address> GetAddressByUserIdAsync(Guid userId);
    Task<Address> UpdateAddressAsync(Guid userId, Address updatedAddress);
    Task<bool> DeleteAddressAsync(Guid userId);
}

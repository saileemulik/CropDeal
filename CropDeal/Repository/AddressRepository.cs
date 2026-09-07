namespace CropDeal.Repository;

public class AddressRepository : IAddressRepository
{
    private readonly CropDealDBContext _context;
    public AddressRepository(CropDealDBContext context)
    {
        _context = context;
    }

    public async Task<Address> AddAddressAsync(Address address)
    {
        address.Id = Guid.NewGuid();
        var existing = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == address.UserId);
        if (existing != null) 
        {
            return existing;

        }
        var user = await _context.Users.FindAsync(address.UserId);
        if (user == null)
        {
            return null;
        }
        address.City = user.Location;
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<Address> GetAddressByUserIdAsync(Guid userId)
    {
        return await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Address> UpdateAddressAsync(Guid userId, Address updatedAddress)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
        if (address == null)
        {
            return null;
        }

        address.Street = updatedAddress.Street;
        address.City = updatedAddress.City;
        address.State = updatedAddress.State;
        address.ZipCode = updatedAddress.ZipCode;

        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<bool> DeleteAddressAsync(Guid userId)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
        if (address == null)
        {
            return false;
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        return true;
    }
}

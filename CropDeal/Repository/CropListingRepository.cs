
namespace CropDeal.Repository;

public class CropListingRepository : ICropListingRepository
{
    private readonly CropDealDBContext _context;
     private readonly IEmailServiceRepository _emailService;

    public CropListingRepository(CropDealDBContext context, IEmailServiceRepository emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    public async Task<IEnumerable<CropListingDto>> GetAllCropsForDealerAsync(Guid dealerId)
    {
        var dealer = await _context.Users.FirstOrDefaultAsync(u => u.Id == dealerId);
        if (dealer == null) return new List<CropListingDto>();

        var dealerAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == dealerId);
        if (dealerAddress == null) return new List<CropListingDto>();

        var listings = await _context.CropListings
            .Include(c => c.Farmer)
            .ToListAsync();

        var farmerIds = listings.Select(l => l.FarmerId).Distinct().ToList();
        var farmerAddresses = await _context.Addresses
            .Where(a => farmerIds.Contains(a.UserId))
            .ToDictionaryAsync(a => a.UserId);

        var result = new List<CropListingDto>();

        foreach (var listing in listings)
        {
            if (!farmerAddresses.TryGetValue(listing.FarmerId, out var farmerAddress))
                continue;

            string category;
            if (string.Equals(farmerAddress.City, dealerAddress.City, StringComparison.OrdinalIgnoreCase))
                category = "City";
            else if (string.Equals(farmerAddress.State, dealerAddress.State, StringComparison.OrdinalIgnoreCase))
                category = "State";
            else
                category = "Other";

            result.Add(new CropListingDto
            {
                Id = listing.Id,
                CropId = listing.CropId,
                FarmerId = listing.FarmerId,
                Description = listing.Description,
                Quantity = listing.Quantity,
                PricePerKg = listing.PricePerKg,
                Unit = listing.Unit,
                Status = listing.Status,
                Location = listing.Location,
                ImageBase64 = listing.ImageUrl != null ? Convert.ToBase64String(listing.ImageUrl) : null,
                LocationCategory = category
            });
        }
        var sortedResult = result.OrderBy(r => r.LocationCategory == "City" ? 0 :
                                           r.LocationCategory == "State" ? 1 : 2)
                            .ThenBy(r => r.CropId)
                            .ToList();

        return sortedResult;
    }



    public async Task<CropListing> GetCropByIdAsync(Guid id)
    {
        Console.WriteLine($"Fetching CropListing with ID: {id}");
         var listing = await _context.CropListings.FindAsync(id);
        Console.WriteLine($"Result using FindAsync: {listing}");

        if (id == Guid.Empty)
        {
            Console.WriteLine("Error: ListingId is empty!");
            return null;
        }
       


        var listings = await _context.CropListings
        .Include(cl => cl.Crop)
        .Include(cl => cl.Farmer)
        .FirstOrDefaultAsync(cl => cl.Id == id);
        Console.WriteLine($"Query Result: {listings}");
        return listings;

    }

    public async Task<CropListing> AddCropAsync(Guid farmerId, CropListing listing)
{
    var crop = await _context.Crops.FindAsync(listing.CropId);
    if (crop == null)
        return null;

    var user = await _context.Users.FindAsync(farmerId);
    if (user == null)
        throw new Exception("Farmer not found");

    bool addressExists = await _context.Addresses.AnyAsync(a => a.UserId == farmerId);
    if (!addressExists)
        throw new Exception("Please add address details");

    bool bankExists = await _context.BankAccounts.AnyAsync(b => b.UserId == farmerId);
    if (!bankExists)
        throw new Exception("Please add bank details");

    listing.Id = Guid.NewGuid();
    listing.FarmerId = farmerId;
    listing.Location = user.Location;
    listing.CreatedAt = DateTime.UtcNow;
    listing.UpdatedAt = DateTime.UtcNow;

    _context.CropListings.Add(listing);
    await _context.SaveChangesAsync();

    // ✅ Now send notifications to subscribed dealers
    var subscribedDealers = await _context.Subscriptions
        .Where(s => s.CropListingId == listing.CropId)
        .Select(s => s.DealerId)
        .ToListAsync();

    foreach (var dealerId in subscribedDealers)
    {
        var dealer = await _context.Users.FindAsync(dealerId);
        if (dealer == null || string.IsNullOrEmpty(dealer.Email))
            continue;

        var message = $"New crop listing available for {crop.Name} by a farmer in your subscription.";

        // Save notification
        var notification = new Notification
        {
            // Id = Guid.NewGuid(),
            UserId = dealerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);

        // Send email
        await _emailService.SendEmailAsync(dealer.Email, "New Crop Listing Notification", message);
    }

    await _context.SaveChangesAsync(); // Save all notifications at once

    return listing;
}


    public async Task UpdateCropAsync(CropListing crop)
    {
        _context.CropListings.Update(crop);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCropAsync(Guid id)
    {
        var crop = await _context.CropListings.FindAsync(id);
        if (crop != null)
        {
            _context.CropListings.Remove(crop);
            await _context.SaveChangesAsync();
        }

    }

    public async Task<IEnumerable<CropListing>> GetAllListingsForAdminAsync()
    {
        return await _context.CropListings
            .Include(c => c.Farmer)
            .Include(c => c.Crop)
            .ToListAsync();
    }

    public IEnumerable<CropListing> GetListingsByUserId(Guid userId)
    {
        return _context.CropListings
            .Where(l => l.FarmerId == userId)
            .ToList();
    }

}

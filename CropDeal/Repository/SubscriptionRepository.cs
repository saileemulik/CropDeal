namespace CropDeal.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly CropDealDBContext _context;

    public SubscriptionRepository(CropDealDBContext context)
    {
        _context = context;
    }
    public async Task<Subscription> AddSubscriptionAsync(Guid dealerId, Guid cropListingId)
    {
        var dealer = await _context.Users.FindAsync(dealerId);
        if (dealer == null)
            throw new Exception("Dealer not found.");
        var existingSubscription = await _context.Subscriptions
            .AnyAsync(s => s.DealerId == dealerId && s.CropListingId == cropListingId);

        if (existingSubscription)
        {
            throw new InvalidOperationException("You are already subscribed to this crop.");
        }

        var cropListing = await _context.CropListings
            .Include(cl => cl.Crop)
            .Include(cl => cl.Farmer)
            .FirstOrDefaultAsync(cl => cl.Id == cropListingId);

        if (cropListing == null)
            throw new Exception("Crop listing not found.");

        if (cropListing.Status == CropAvailability.OutOfStock)
        {
            throw new InvalidOperationException("Crop is out of stock");
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            DealerId = dealerId,
            CropListingId = cropListingId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        var notification = new Notification
        {
            UserId = cropListing.FarmerId,
            Message = $"Dealer {dealer.Name} subscribed to your crop: {cropListing.Crop?.Name ?? "Unknown Crop"}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return subscription;
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionsByDealerIdAsync(Guid dealerId)
    {
        return await _context.Subscriptions
            .Where(s => s.DealerId == dealerId)
            .Include(s => s.CropListing)
                .ThenInclude(cl => cl.Crop)
            .Include(s => s.Dealer) // Include Dealer to get Dealer.Name
            .ToListAsync();
    }


    public async Task<bool> DeleteSubscriptionAsync(Guid dealerId, Guid cropListingId)
    {
        var dealer = await _context.Users.FindAsync(dealerId);
        var subscription = await _context.Subscriptions
            .Include(s => s.CropListing)
                .ThenInclude(cl => cl.Crop)
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.CropListingId == cropListingId);

        if (subscription == null)
        {
            return false;
        }

        // Get the farmerId from the crop listing
        var farmerId = subscription.CropListing.FarmerId;

        // Remove the subscription
        _context.Subscriptions.Remove(subscription);

        // Create the notification
        var notification = new Notification
        {
            UserId = farmerId,
            Message = $"Dealer {dealer.Name} unsubscribed from your crop: {subscription.CropListing.Crop.Name}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        // Save changes to both
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<IEnumerable<SubscriptionDto>> GetSubscribedListingsForFarmerAsync(Guid farmerId)
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.CropListing.FarmerId == farmerId)
            .Include(s => s.CropListing)
                .ThenInclude(cl => cl.Crop)
            .Include(s => s.Dealer)  // Assuming Dealer is linked to the User model
            .Select(s => new
            {
                s.CropListingId,
                Crop = new
                {
                    s.CropListing.Crop.Id,
                    s.CropListing.Crop.Name,
                    s.CropListing.Crop.Type
                },
                DealerId = s.Dealer.Id
            })
            .ToListAsync();

        if (subscriptions == null || !subscriptions.Any())
        {
            return new List<SubscriptionDto>();
        }

        var dealerIds = subscriptions.Select(sub => sub.DealerId).Distinct().ToList();
        var dealerNames = await _context.Users
            .Where(u => dealerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Name })
            .ToListAsync();

        var result = subscriptions.Select(sub => new SubscriptionDto
        {
            CropListingId = sub.CropListingId ?? Guid.Empty,
            Crop = new Crop
            {
                Id = sub.Crop.Id,
                Name = sub.Crop.Name,
                Type = sub.Crop.Type
            },

        }).ToList();

        return result;
    }


    public async Task<Subscription> AddCropNameSubscriptionAsync(Guid dealerId, string cropName)
    {
        try
        {
            // Check if subscription already exists (optional but recommended)
            var existing = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.CropName == cropName);
            if (existing != null)
                throw new InvalidOperationException("You are already subscribed to this crop name.");

            // Create new subscription entity
            var subscription = new Subscription
            {
                DealerId = dealerId,
                CropName = cropName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }
        catch (DbUpdateException dbEx)
        {
            // Log dbEx.InnerException.Message for details
            throw new Exception("Database error while adding subscription: " + dbEx.InnerException?.Message, dbEx);
        }
    }

    public async Task<bool> DeleteCropNameSubscriptionAsync(Guid dealerId, string cropName)
    {
        var normalizedCropName = cropName.Trim().ToLower();

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s =>
                s.DealerId == dealerId &&
                s.CropName.ToLower() == normalizedCropName &&
                s.CropListingId == null); // important!

        if (subscription == null)
            return false;

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<string>> GetCropNameSubscriptionsByDealerAsync(Guid dealerId)
    {
        return await _context.Subscriptions
            .Where(s => s.DealerId == dealerId && s.CropListingId == null && s.CropName != null)
            .Select(s => s.CropName )
            .ToListAsync();
    }



}



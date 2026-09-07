using Microsoft.EntityFrameworkCore;
// using CropDeal.Models.Domain;

public class NegotiationRepository : INegotiationRepository
{
    private readonly CropDealDBContext _context;

    public NegotiationRepository(CropDealDBContext context)
    {
        _context = context;
    }

    public async Task<PriceNegotiationRequest?> GetNegotiationByDealerAndListingAsync(Guid dealerId, Guid listingId)
    {
        return await _context.PriceNegotiations
            .FirstOrDefaultAsync(n => n.DealerId == dealerId && n.ListingId == listingId && n.Status ==  PriceStatus.Accepted);
    }
}

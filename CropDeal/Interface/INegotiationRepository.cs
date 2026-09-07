using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Interface
{
    public interface INegotiationRepository
    {
        Task<PriceNegotiationRequest?> GetNegotiationByDealerAndListingAsync(Guid dealerId, Guid listingId);
    }
}
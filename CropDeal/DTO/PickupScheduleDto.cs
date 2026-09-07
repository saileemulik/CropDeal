using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.DTO
{
    public class PickupScheduleDto
    {
        public Guid ListingId { get; set; }
    public Guid FarmerId { get; set; }
    public Guid DealerId { get; set; }
    public DateTime ProposedPickupSlot { get; set; }
    }
}
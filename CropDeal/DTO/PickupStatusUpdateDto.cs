using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.DTO
{
    public class PickupStatusUpdateDto
    {
         public Guid PickupId { get; set; }
    public PickupStatus Status { get; set; }
    }
}
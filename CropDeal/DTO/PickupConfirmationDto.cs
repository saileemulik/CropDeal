using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.DTO
{
    public class PickupConfirmationDto
    {
         public Guid PickupId { get; set; }
    public DateTime ConfirmedPickupSlot { get; set; }
    }
}
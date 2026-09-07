using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Interface
{
    public interface IPickupScheduleRepository
    {
        Task<PickupSchedule> CreateProposalAsync(PickupSchedule schedule);

        Task<bool> ConfirmPickupSlotAsync(Guid scheduleId, DateTime confirmedSlot);

        Task<bool> UpdatePickupStatusAsync(Guid scheduleId, PickupStatus newStatus);

        Task<IEnumerable<PickupSchedule>> GetPickupByListingIdAsync(Guid listingId);

        Task<PickupSchedule> GetByIdAsync(Guid id);

        Task<bool> UpdateProposedSlotAsync(Guid id, DateTime newProposedSlot);
        Task<bool> CancelProposalAsync(Guid id);

    }
    
}
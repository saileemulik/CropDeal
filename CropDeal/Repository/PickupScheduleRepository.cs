using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Repository
{
    public class PickupScheduleRepository : IPickupScheduleRepository
    {
        private readonly CropDealDBContext _context;

        public PickupScheduleRepository(CropDealDBContext context)
        {
            _context = context;
        }
        public async Task<PickupSchedule> CreateProposalAsync(PickupSchedule schedule)
        {
            await _context.PickupSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> ConfirmPickupSlotAsync(Guid scheduleId, DateTime confirmedSlot)
        {
            var schedule = await _context.PickupSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            schedule.ConfirmedPickupSlot = confirmedSlot;
            schedule.Status = PickupStatus.Scheduled;
            _context.PickupSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePickupStatusAsync(Guid scheduleId, PickupStatus newStatus)
        {
            var schedule = await _context.PickupSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            schedule.Status = newStatus;

            // ✅ If status is Confirmed, set ConfirmedPickupSlot = ProposedPickupSlot
            if (newStatus == PickupStatus.Confirmed && schedule.ProposedPickupSlot != null)
            {
                schedule.ConfirmedPickupSlot = schedule.ProposedPickupSlot;
            }

            _context.PickupSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PickupSchedule>> GetPickupByListingIdAsync(Guid listingId)
        {
            return await _context.PickupSchedules
                .Where(p => p.ListingId == listingId)
                .ToListAsync();
        }

        public async Task<PickupSchedule> GetByIdAsync(Guid id)
        {
            return await _context.PickupSchedules.FindAsync(id);
        }

        public async Task<bool> UpdateProposedSlotAsync(Guid id, DateTime newProposedSlot)
        {
            Console.WriteLine("Trying to Update");
            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule == null)
            {
                Console.WriteLine("Schedule not found.");
                return false;
            }

            Console.WriteLine($"Schedule found with status: {schedule.Status}");

            if (schedule.Status != PickupStatus.Proposed)
            {
                Console.WriteLine("Schedule is not in Proposed status.");
                return false;
            }

            schedule.ProposedPickupSlot = newProposedSlot;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelProposalAsync(Guid id)
        {
            var schedule = await _context.PickupSchedules.FindAsync(id);
            if (schedule == null || schedule.Status != PickupStatus.Proposed) return false;

            _context.PickupSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
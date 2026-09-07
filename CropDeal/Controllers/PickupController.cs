using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CropDeal.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PickupController : ControllerBase
    {
        private readonly IPickupScheduleRepository _pickupRepo;
        private readonly CropDealDBContext _context;
        private readonly IEmailServiceRepository _emailService;

        public PickupController(IPickupScheduleRepository pickupRepo, CropDealDBContext context, IEmailServiceRepository emailService)
        {
            _pickupRepo = pickupRepo;
            _context = context;
            _emailService = emailService;
        }

        // POST: api/PickupSchedule/propose
       [Authorize(Roles = "Dealer")]
[HttpPost("propose")]
public async Task<IActionResult> CreateProposal([FromBody] PickupSchedule schedule)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // ✅ Get CropListing to fetch FarmerId
    var cropListing = await _context.CropListings.FindAsync(schedule.ListingId);
    if (cropListing == null)
        return NotFound(new { message = "Invalid listingId. CropListing not found." });

    schedule.FarmerId = cropListing.FarmerId;

    // ✅ Save the pickup proposal
    var result = await _pickupRepo.CreateProposalAsync(schedule);

    // ✅ Notify the farmer
    var farmer = await _context.Users.FindAsync(schedule.FarmerId);
    if (farmer != null)
    {
        string message = $"A dealer has proposed a pickup for your crop listing '{cropListing.CropId}' on {schedule.ProposedPickupSlot.ToString("f")}.";

        // Save notification
        var notification = new Notification
        {
            UserId = schedule.FarmerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Send email to farmer
        if (!string.IsNullOrEmpty(farmer.Email))
        {
            await _emailService.SendEmailAsync(
                farmer.Email,
                "New Pickup Proposal",
                message
            );
        }
    }

    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}



        // PUT: api/PickupSchedule/confirm/{id}
        [Authorize(Roles = "Farmer")]
[HttpPut("confirm/{id}")]
public async Task<IActionResult> ConfirmPickupSlot(Guid id)
{
    var schedule = await _context.PickupSchedules
        .Include(p => p.Dealer) // To get dealer info
        .FirstOrDefaultAsync(p => p.Id == id);

    if (schedule == null)
        return NotFound("Pickup schedule not found.");

    // Confirm the proposed slot
    schedule.ConfirmedPickupSlot = schedule.ProposedPickupSlot;
    schedule.Status = PickupStatus.Confirmed;

    _context.PickupSchedules.Update(schedule);
    await _context.SaveChangesAsync();

    // ✅ Send notification + email to the dealer
    if (schedule.DealerId != Guid.Empty)
    {
        var message = $"Your pickup slot for listing '{schedule.ListingId}' has been confirmed by the farmer for {schedule.ConfirmedPickupSlot?.ToString("f")}.";

        // Save notification and send email
        var notification = new Notification
        {
            UserId = schedule.DealerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var dealer = await _context.Users.FindAsync(schedule.DealerId);
        if (dealer != null && !string.IsNullOrWhiteSpace(dealer.Email))
        {
            await _emailService.SendEmailAsync(
                dealer.Email,
                "Pickup Slot Confirmed",
                message
            );
        }
    }

    return Ok(schedule);
}



        // PUT: api/PickupSchedule/status/{id}
        [Authorize(Roles = "Farmer")]
        [HttpPut("status/{scheduleId}")]
        public async Task<IActionResult> UpdateStatus(Guid scheduleId, [FromBody] string newStatus)
        {
            if (!Enum.TryParse<PickupStatus>(newStatus, out var parsedStatus))
                return BadRequest(new {message ="Invalid pickup status."});

            var result = await _pickupRepo.UpdatePickupStatusAsync(scheduleId, parsedStatus);

            if (!result)
                return NotFound();

            return Ok(); 
        }





        // GET: api/PickupSchedule/listing/{listingId}
        [Authorize(Roles = "Dealer, Farmer")]
        [HttpGet("listing/{listingId}")]
        public async Task<ActionResult<IEnumerable<PickupSchedule>>> GetPickupByListing(Guid listingId)
        {
            var pickups = await _pickupRepo.GetPickupByListingIdAsync(listingId);
            return Ok(pickups);
        }

        // GET: api/PickupSchedule/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PickupSchedule>> GetById(Guid id)
        {
            var pickup = await _pickupRepo.GetByIdAsync(id);
            if (pickup == null)
                return NotFound();

            return Ok(pickup);
        }

        [Authorize(Roles = "Dealer")]
        [HttpPut("update-proposal/{id}")]
        public async Task<IActionResult> UpdateProposedSlot(Guid id, [FromBody] DateTime newProposedSlot)
        {
            var success = await _pickupRepo.UpdateProposedSlotAsync(id, newProposedSlot);
            Console.WriteLine($"Trying to update pickup schedule with ID: {id}");

            if (!success)
                return NotFound(new { message = $"No schedule found with ID: {id}" });

            return Ok(new {message ="Proposed slot updated successfully."});
        }

        // DELETE: api/PickupSchedule/cancel/{id}
        [Authorize(Roles = "Dealer")]
        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelProposal(Guid id)
        {
            var success = await _pickupRepo.CancelProposalAsync(id);
            if (!success)
                return NotFound(new {message =$"No schedule found with ID: {id}"});

            return Ok( new { message ="Pickup proposal cancelled."});
        }
    }
}
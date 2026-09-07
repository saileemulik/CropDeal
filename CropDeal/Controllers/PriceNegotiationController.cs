using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CropDeal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PriceNegotiationController : ControllerBase
    {
        private readonly CropDealDBContext _context;
        private readonly IEmailServiceRepository _emailService;

        public PriceNegotiationController(CropDealDBContext context, IEmailServiceRepository emailService)
        {
            _context = context;
            _emailService = emailService;
        }

  [HttpPost]
public async Task<IActionResult> CreateNegotiation([FromBody] PriceNegotiationRequestDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var negotiation = new PriceNegotiationRequest
    {
        Id = Guid.NewGuid(),
        ListingId = dto.ListingId,
        DealerId = dto.DealerId,
        FarmerId = dto.FarmerId,
        Quantity = dto.Quantity,
        NegotiatedPricePerKg = dto.NegotiatedPricePerKg,
        TotalPrice = dto.TotalPrice,
        Status = PriceStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    _context.PriceNegotiations.Add(negotiation);
    await _context.SaveChangesAsync();

    // ✅ Notify the farmer
    var farmer = await _context.Users.FindAsync(dto.FarmerId);
    if (farmer != null)
    {
        string message = $"New price negotiation request received for your crop listing (Qty: {dto.Quantity} kg at ₹{dto.NegotiatedPricePerKg}/kg).";

        var notification = new Notification
        {
            UserId = dto.FarmerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(farmer.Email))
        {
            await _emailService.SendEmailAsync(
                farmer.Email,
                "New Price Negotiation",
                message
            );
        }
    }

    return Ok(negotiation);
}


    
    [HttpGet("farmer/{farmerId}")]
    public async Task<IActionResult> GetNegotiationsForFarmer(Guid farmerId)
    {
        var negotiations = await _context.PriceNegotiations
            .Where(n => n.FarmerId == farmerId )
            .ToListAsync();

        return Ok(negotiations);
    }

    
    [HttpPut("{id}/accept")]
public async Task<IActionResult> AcceptNegotiation(Guid id)
{
    var negotiation = await _context.PriceNegotiations.FindAsync(id);
    if (negotiation == null) return NotFound();

    negotiation.Status = PriceStatus.Accepted;
    negotiation.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    // ✅ Notify the dealer
    var dealer = await _context.Users.FindAsync(negotiation.DealerId);
    if (dealer != null)
    {
        string message = $"Your price negotiation request has been accepted by the farmer.";

        var notification = new Notification
        {
            UserId = dealer.Id,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(dealer.Email))
        {
            await _emailService.SendEmailAsync(
                dealer.Email,
                "Negotiation Accepted",
                message
            );
        }
    }

    return Ok(negotiation);
}


    
    [HttpPut("{id}/reject")]
public async Task<IActionResult> RejectNegotiation(Guid id)
{
    var negotiation = await _context.PriceNegotiations.FindAsync(id);
    if (negotiation == null) return NotFound();

    negotiation.Status = PriceStatus.Rejected;
    negotiation.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    // ✅ Notify the dealer
    var dealer = await _context.Users.FindAsync(negotiation.DealerId);
    if (dealer != null)
    {
        string message = $"Your price negotiation request has been rejected by the farmer.";

        var notification = new Notification
        {
            UserId = dealer.Id,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(dealer.Email))
        {
            await _emailService.SendEmailAsync(
                dealer.Email,
                "Negotiation Rejected",
                message
            );
        }
    }

    return Ok(negotiation);
}


    
    [HttpGet("dealer/{dealerId}")]
    public async Task<IActionResult> GetNegotiationsForDealer(Guid dealerId)
    {
        var negotiations = await _context.PriceNegotiations
            .Where(n => n.DealerId == dealerId)
            .ToListAsync();

        return Ok(negotiations);
    }
    }
}
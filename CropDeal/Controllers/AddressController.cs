namespace CropDeal.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _addressRepo;

    public AddressController(IAddressRepository addressRepo)
    {
        _addressRepo = addressRepo;
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] Address address)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        address.UserId = userId;

        var result = await _addressRepo.AddAddressAsync(address);
        if(result == null)
        {
            return BadRequest();
        }
        return Ok(new {result});
    }

    [HttpGet]
    public async Task<IActionResult> GetAddress()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _addressRepo.GetAddressByUserIdAsync(userId);

        if (result == null)
        {
            return NotFound(new {message = "Address not found"});
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAddress([FromBody] Address address)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _addressRepo.UpdateAddressAsync(userId, address);

        if (result == null)
        {
            return NotFound(new { message = "Address not found" });
        }

        return Ok(new { result });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAddress()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var success = await _addressRepo.DeleteAddressAsync(userId);

        if (!success) 
        {
            return NotFound(new {message = "Address not found"});
        }

        return Ok(new { message = "Address deleted successfully"});
    }
}

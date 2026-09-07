using DocumentFormat.OpenXml.Office2010.Excel;

namespace CropDeal.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankAccountController : ControllerBase
{
    private readonly IBankAccountRepository _bankRepo;

    public BankAccountController(IBankAccountRepository bankRepo)
    {
        _bankRepo = bankRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var accounts = await _bankRepo.GetBankAccountsAsync(userId);
        return Ok(new { accounts });
    }

    [HttpPost]
    public async Task<IActionResult> AddAccount([FromBody] BankAccount account)
    {
        account.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var added = await _bankRepo.AddBankAccountAsync(account);
        account.CreatedAt = DateTime.UtcNow;
        return Ok(new { added });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBankAccount([FromBody] BankAccount account)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _bankRepo.UpdateBankAccountAsync(userId, account);

        if (result == null)
        {
            return NotFound(new { message = "Bank Account not found" });
        }

        return Ok(new {result});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bankRepo.DeleteBankAccountAsync(id);
        if (!result)
        {
            return NotFound("Account not found");
        }
        return Ok("Deleted successfully");
    }
}

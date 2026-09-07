namespace CropDeal.Repository;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly CropDealDBContext _context;
    public BankAccountRepository(CropDealDBContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<BankAccount>> GetBankAccountsAsync(Guid userId)
    {
        return await _context.BankAccounts.Where(b => b.UserId == userId).ToListAsync();
    }
    public async Task<BankAccount> AddBankAccountAsync(BankAccount account)
    {
        account.Id = Guid.NewGuid();
        var user = await _context.Users.FindAsync(account.UserId);
        if (user != null)
        {
            // user.BankAccountId = account.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        account.CreatedAt = DateTime.UtcNow;
        _context.BankAccounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }


    public async Task<BankAccount> GetBankAccountByUserIdAsync(Guid userId)
    {
        return await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> DeleteBankAccountAsync(Guid userId)
    {
        var account = await _context.BankAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
        if (account == null)
        {
            return false;
        }

        _context.BankAccounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BankAccount> UpdateBankAccountAsync(Guid userId, BankAccount updatedAccount)
    {
        var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {
            return null;
        }
        account.AccountNumber = updatedAccount.AccountNumber;
        account.BankName = updatedAccount.BankName;
        account.BranchName = updatedAccount.BranchName;
        account.IFSCCode = updatedAccount.IFSCCode;
        await _context.SaveChangesAsync();
        return account;
    }

}

namespace CropDeal.Interface;

public interface IBankAccountRepository
{
    Task<IEnumerable<BankAccount>> GetBankAccountsAsync(Guid userId);
    Task<BankAccount> AddBankAccountAsync(BankAccount bankAccount);
    Task<BankAccount> GetBankAccountByUserIdAsync(Guid userId);
    Task<BankAccount> UpdateBankAccountAsync(Guid userId,BankAccount bankAccount);
    Task<bool> DeleteBankAccountAsync(Guid userId);
}

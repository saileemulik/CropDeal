namespace CropDeal.Interface;

public interface ITransactionRepository
{
    // Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction> GetByIdAsync(Guid id);
    //  Task<CropListing> GetCropByIdAsync(Guid listingId);
    Task<Transaction> AddTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetTransactionsByDealerIdAsync(Guid dealerId);
    Task<IEnumerable<Transaction>> GetTransactionsByFarmerIdAsync(Guid farmerId);
    Task UpdateAsync(Transaction transaction);
    Task<bool> UpdateTransactionStatusAsync(Guid id, TransactionStatus status);
    Task<bool> VerifyTransactionAndUpdateListingAsync(string orderId);
    Task SaveChangesAsync();
    Task<IEnumerable<(Guid TransactionId, Guid FarmerId)>> GetFarmerIdsByDealerIdAsync(Guid dealerId);
    Task<bool> UpdateOrderIdAsync(Guid transactionId, string orderId);
    Task<Transaction> GetByDealerAndListingAsync(Guid dealerId, Guid listingId);

}

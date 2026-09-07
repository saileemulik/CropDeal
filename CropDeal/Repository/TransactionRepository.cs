namespace CropDeal.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly CropDealDBContext _context;
    public TransactionRepository(CropDealDBContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDealerIdAsync(Guid dealerId)
    {
        return await _context.Transactions.Where(t => t.DealerId == dealerId).ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByFarmerIdAsync(Guid farmerId)
    {
        return await _context.Transactions.Where(t => t.FarmerId == farmerId).ToListAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }




    public async Task UpdateAsync(Transaction transaction)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transaction.Id);
        if (existingTransaction == null)
            throw new Exception("Transaction not found for update");

        _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        Console.WriteLine("Entering CreateTransaction method...");
        transaction.Id = Guid.NewGuid();
        var dealer = await _context.Users.FindAsync(transaction.DealerId);
        var cropListing = await _context.CropListings.FindAsync(transaction.ListingId);
        var farmer = await _context.Users.FindAsync(cropListing.FarmerId);

        if (dealer == null)
        {
            Console.WriteLine("Dealer is missing.");
            return null;
        }
        if (cropListing == null)
        {
            Console.WriteLine("CropListing is missing.");
            return null;
        }
        if (farmer == null)
        {
            Console.WriteLine("Farmer is missing.");
            return null;
        }
        transaction.FarmerId = cropListing.FarmerId;

        var isSubscribed = await _context.Subscriptions.AnyAsync(s => s.DealerId == transaction.DealerId && s.CropListingId == transaction.ListingId);

        if (!isSubscribed)
        {
            Console.WriteLine("Not subscribed.");
            return null;
        }

        if (cropListing.Status != CropAvailability.Available || transaction.Quantity > cropListing.Quantity)
        {
            Console.WriteLine("Satus and quantity is missing.");
            return null;
        }

        // Calculate the total price
        transaction.TotalPrice = cropListing.PricePerKg * transaction.Quantity;

        // Set default status
        transaction.Status = TransactionStatus.Pending;
        Console.WriteLine($"Received OrderId: {transaction.OrderId}, Quantity: {transaction.Quantity} {transaction.Currency}");

        // Ensure PaymentGateway and OrderId are set (should be passed from the frontend or payment order process)
        if (string.IsNullOrEmpty(transaction.PaymentGateway))
        {
            Console.WriteLine("PaymentGateway is missing.");
            return null; // If PaymentGateway is not set, return null (error)
        }
        if (string.IsNullOrEmpty(transaction.Currency))
        {
            Console.WriteLine("Currency is missing.");
            return null; // If PaymentGateway is not set, return null (error)
        }

        if (string.IsNullOrEmpty(transaction.OrderId))
        {
            Console.WriteLine("OrderId is missing.");
            return null; // If OrderId is not set, return null (error)
        }

        // Ensure unique PaymentId and timestamps
        // transaction.PaymentId = Guid.NewGuid();


        transaction.CreatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = DateTime.UtcNow;

        try
        {
            // Add transaction to the database
            await _context.Transactions.AddAsync(transaction);
            Console.WriteLine("Saving transaction...");
            await _context.SaveChangesAsync();
            Console.WriteLine("Transaction saved.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transaction: {ex.Message}");
            return null; // In case of error, return null
        }

        return transaction; // Return the created transaction
    }

    public async Task<bool> UpdateTransactionStatusAsync(Guid id, TransactionStatus status)
    {
        var txn = await _context.Transactions.FindAsync(id);
        if (txn == null)
        {
            return false;
        }

        txn.Status = status;
        txn.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<(Guid TransactionId, Guid FarmerId)>> GetFarmerIdsByDealerIdAsync(Guid dealerId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.DealerId == dealerId && t.Status == TransactionStatus.Completed)
            .ToListAsync();

        if (!transactions.Any())
        {
            return null;
        }

        var dealerExists = await _context.Users.AnyAsync(u => u.Id == dealerId);
        if (!dealerExists)
        {
            return null;
        }

        var result = transactions.Select(t => (t.Id, t.FarmerId)).ToList();

        foreach (var transaction in transactions)
        {
            var cropListing = await _context.CropListings
                .FirstOrDefaultAsync(c => c.Id == transaction.ListingId);

            if (cropListing != null)
            {
                cropListing.Quantity -= transaction.Quantity;
                if (cropListing.Quantity < 0)
                {
                    cropListing.Quantity = 0;
                    cropListing.Status = CropAvailability.OutOfStock;
                }
            }
            _context.Update(cropListing);
        }


        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<bool> UpdateOrderIdAsync(Guid transactionId, string orderId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null)
        {
            Console.WriteLine($"Transaction not found with Id: {transactionId}");
            return false;
        }

        transaction.OrderId = orderId;
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Updated OrderId for transaction {transactionId} to {orderId}");
        return true;
    }

    public async Task<bool> VerifyTransactionAndUpdateListingAsync(string orderId)
    {
        // Find the transaction by OrderId
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.OrderId == orderId);
        if (transaction == null)
        {
            Console.WriteLine($"[VerifyTransactionAndUpdateListingAsync] Transaction not found for Order ID: {orderId}");
            return false;
        }

        Console.WriteLine($"[VerifyTransactionAndUpdateListingAsync] Transaction found: ID = {transaction.Id}, OrderID = {transaction.OrderId}, ListingID = {transaction.ListingId}");

        // Mark the transaction as completed
        transaction.Status = TransactionStatus.Completed;
        transaction.UpdatedAt = DateTime.UtcNow;

        // Retrieve the associated crop listing
        var cropListing = await _context.CropListings.FirstOrDefaultAsync(c => c.Id == transaction.ListingId);
        if (cropListing == null)
        {
            Console.WriteLine($"[VerifyTransactionAndUpdateListingAsync] CropListing not found for Listing ID: {transaction.ListingId}");
            return false;
        }

        // Adjust the quantity and availability status
        cropListing.Quantity -= transaction.Quantity;
        if (cropListing.Quantity <= 0)
        {
            cropListing.Quantity = 0;
            cropListing.Status = CropAvailability.OutOfStock;
        }

        // Mark entities for update
        _context.Transactions.Update(transaction);
        _context.CropListings.Update(cropListing);

        // Save changes and return true if successful
        var result = await _context.SaveChangesAsync();
        Console.WriteLine($"[VerifyTransactionAndUpdateListingAsync] SaveChangesAsync result: {result}");

        return result > 0;
    }
    
     public async Task<Transaction> GetByDealerAndListingAsync(Guid dealerId, Guid listingId)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.DealerId == dealerId && t.ListingId == listingId);
    }



}

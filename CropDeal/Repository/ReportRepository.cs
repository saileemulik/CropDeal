namespace CropDeal.Repository;

public class ReportRepository : IReportRepository
{
    private readonly CropDealDBContext _context;

    public ReportRepository(CropDealDBContext context)
    {
        _context = context;
    }

    public async Task<Report> GenerateDealerReportAsync(Guid adminId, Guid dealerId, string filter = null)
    {
        var query = _context.Transactions
            .Where(t => t.DealerId == dealerId)
            .Select(t => new
            {
                DealerName = t.Dealer.Name,
                CropName = t.Listing.Crop.Name,
                Quantity = t.Quantity.ToString(),
                TotalPrice = t.TotalPrice,
                TransactionDate = t.CreatedAt,
                Status = t.Status.ToString()
            }).AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(t => t.Status.ToString().Contains(filter));
        }

        var transactions = await query.ToListAsync();

        var content = string.Join(Environment.NewLine, transactions.Select(t =>
            $"Dealer Name: {t.DealerName}, Crop Name: {t.CropName}, Quantity: {t.Quantity}, Price: ₹{t.TotalPrice}, Date: {t.TransactionDate:yyyy-MM-dd}, Status: {t.Status}"));
        var report = new Report
        {
            Id = Guid.NewGuid(),
            Title = $"Report for Dealer {dealerId}",
            Content = content,
            GeneratedBy = adminId,
            GeneratedFor = dealerId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return report;
    }
    public async Task<IEnumerable<Report>> GetAllAsync()
    {
        return await _context.Reports.ToListAsync();
    }

    public async Task<Report> GetByIdAsync(Guid id)
    {
        return await _context.Reports.FindAsync(id);
    }

    public async Task<Report> UpdateAsync(Guid id, Report updatedReport)
    {
        var existing = await _context.Reports.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Title = updatedReport.Title;
        existing.Content = updatedReport.Content;

        _context.Reports.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return false;

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }

}

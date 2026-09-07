namespace CropDeal.Interface;

public interface IReportRepository
{
    Task<Report> GenerateDealerReportAsync(Guid adminId, Guid dealerId, string filter = null);
    Task<IEnumerable<Report>> GetAllAsync();
    Task<Report> GetByIdAsync(Guid id);
    Task<Report> UpdateAsync(Guid id, Report report);
    Task<bool> DeleteAsync(Guid id);

}

namespace CropDeal.Interface;

public interface ICropRepository
{
    Task<IEnumerable<Crop>> GetAllAsync();
    Task<Crop> GetByIdAsync(Guid id);
    Task AddAsync(Crop crop, CropTypeEnum type);
    Task UpdateAsync(Crop crop, CropTypeEnum type);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Crop>> SearchByNameAsync(string name);
}

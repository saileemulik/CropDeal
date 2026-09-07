namespace CropDeal.Repository;
public class CropRepository : ICropRepository
{
    private readonly CropDealDBContext _context;

    public CropRepository(CropDealDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Crop>> GetAllAsync()
    {
        return await _context.Crops.ToListAsync();
    }

    public async Task<Crop> GetByIdAsync(Guid id)
    {
        return await _context.Crops.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Crop crop, CropTypeEnum type)
    {
        var cropExist = await _context.Crops.FirstOrDefaultAsync(c => c.Name == crop.Name);;
        if(cropExist != null)
        {
           throw new Exception("Crop Already Exists");
        }
        crop.Id = Guid.NewGuid();
        crop.Type = type;
        crop.CreatedAt = DateTime.UtcNow;
        crop.UpdatedAt = DateTime.UtcNow;
        _context.Crops.Add(crop);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Crop updatedCrop, CropTypeEnum type)
    {
        var existingCrop = await _context.Crops.FindAsync(updatedCrop.Id);
        if (existingCrop == null)
        {
            throw new ArgumentException("Crop not found.");
        }

        existingCrop.Name = updatedCrop.Name;
        existingCrop.Type = type;
        existingCrop.UpdatedAt = DateTime.UtcNow;

        _context.Crops.Update(existingCrop);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(Guid id)
    {
        var crop = await _context.Crops.FindAsync(id);
        if (crop != null)
        {
            _context.Crops.Remove(crop);
            await _context.SaveChangesAsync();
        }
    }
public async Task<IEnumerable<Crop>> SearchByNameAsync(string name)
{
    return await _context.Crops
        .Where(c => c.Name.Contains(name))
        .ToListAsync();
}

}

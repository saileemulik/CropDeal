namespace CropDeal.Repository;

public class AdminRepository : IAdminRepository
{
    private readonly CropDealDBContext _context;
    public AdminRepository(CropDealDBContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users.Where(u => u.Role == UserRole.Farmer || u.Role == UserRole.Dealer)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Location = u.Location,
                Role = u.Role,
                Status = u.Status,
                AverageRating = u.AverageRating,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToListAsync();
    }

    public async Task<User> GetUserEntityByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

   public async Task<User?> UpdateUserEntityAsync(Guid userId, AdminUpdateDto userDto)
{
    var existingUser = await GetUserEntityByIdAsync(userId);
    if (existingUser == null)
    {
        return null;
    }

    existingUser.Status = userDto.Status;
    existingUser.AverageRating = userDto.AverageRating;

    
    await _context.SaveChangesAsync();

    return existingUser;
}


    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

}

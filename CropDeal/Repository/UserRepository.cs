namespace CropDeal.Repository;

public class UserRepository : IUserRepository
{
    private readonly CropDealDBContext _context;

    public UserRepository(CropDealDBContext context)
    {
        _context = context;
    }
    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);


        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Location = user.Location,
            AverageRating = user.AverageRating,
            Role = user.Role,
            Status = user.Status,
            // BankAccountId = user.BankAccountId ?? Guid.Empty,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

  
    public async Task<User> GetUserByIdAsModelAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

}

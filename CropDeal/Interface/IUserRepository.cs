namespace CropDeal.Interface;
public interface IUserRepository
{
    // Task<IEnumerable<User>> GetAllUserssAsync();
    Task<UserDto> GetUserByIdAsync(Guid id);
    // Task AddUserAsync(User user);
    Task<User> GetUserByIdAsModelAsync(Guid id);
    Task UpdateUserAsync(User user);
    // Task DeleteUserAsync(Guid id);
}

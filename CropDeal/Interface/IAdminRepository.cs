namespace CropDeal.Interface;

public interface IAdminRepository
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<User> GetUserEntityByIdAsync(Guid id);
    Task<User?> UpdateUserEntityAsync(Guid userId, AdminUpdateDto userDto);
    Task<bool> DeleteUserAsync(Guid id);

}
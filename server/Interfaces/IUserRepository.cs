using server.Entities;
using server.Helpers;

namespace server.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<PagedList<AppUser>> GetUsersAsync(UserParams userParamss);
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
}
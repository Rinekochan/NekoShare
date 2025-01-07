using server.DTOs.User;
using server.Entities;

namespace server.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUser(UserUpdateDto userDto, string username);
}
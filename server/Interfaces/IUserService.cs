using server.DTOs.Photo;
using server.DTOs.User;
using server.Entities;

namespace server.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUser(string username, UserUpdateDto userDto);
    Task<PhotoDto> AddPhoto(string username, IFormFile file);
    Task<bool> SetMainPhoto(string username, int photoId);
    Task<bool> DeletePhoto(string username, int photoId);
}
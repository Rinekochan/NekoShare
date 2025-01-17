using server.DTOs.Photo;
using server.DTOs.User;
using server.Entities;
using server.Helpers;

namespace server.Interfaces;

public interface IUserService
{
    Task<PagedList<UserResponseDto>> GetUsersAsync(UserParams userParams);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto?> GetUserByUsernameAsync(string username, string? currentUser = null);
    Task<UserResponseDto?> GetUserByPhotoId(int photoId);
    Task<bool> UpdateUser(string username, UserUpdateDto userDto);
    Task<PhotoDto> AddPhoto(string username, IFormFile file);
    Task<bool> SetMainPhoto(string username, int photoId);
    Task<bool> DeletePhoto(string username, int photoId);
}
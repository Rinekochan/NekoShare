using AutoMapper;
using server.DTOs.Photo;
using server.DTOs.User;
using server.Entities;
using server.Enums;
using server.Exceptions;
using server.Interfaces;
using Photo = server.Entities.Photo;

namespace server.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : IUserService
{
    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        IEnumerable<AppUser> users = await userRepository.GetUsersAsync();
        return mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        AppUser? user = await userRepository.GetUserByIdAsync(id);
        return mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> GetUserByUsernameAsync(string username)
    {
        AppUser? user = await userRepository.GetUserByUsernameAsync(username);
        return mapper.Map<UserResponseDto>(user);    
    }
    
    public async Task<bool> UpdateUser(string username, UserUpdateDto userDto)
    {
        AppUser user = await userRepository.GetUserByUsernameAsync(username) 
                        ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);
        
        mapper.Map(userDto, user);
        return await userRepository.SaveAllAsync();
    }
    
    public async Task<PhotoDto> AddPhoto(string username, IFormFile file)
    {
        AppUser user = await userRepository.GetUserByUsernameAsync(username) 
                        ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) throw new Exception(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        
        user.Photos.Add(photo);

        try
        {
            await userRepository.SaveAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add photo");
        }

        return mapper.Map<PhotoDto>(photo);
    }

    public async Task<bool> SetMainPhoto(string username, int photoId)
    {
        AppUser user = await userRepository.GetUserByUsernameAsync(username) 
                       ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);

        Photo photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId)
                      ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);
        
        if (photo.IsMain) throw new Exception("This photo is already your main photo");

        Photo? currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        return await userRepository.SaveAllAsync();
    }

    public async Task<bool> DeletePhoto(string username, int photoId)
    {
        AppUser user = await userRepository.GetUserByUsernameAsync(username) 
                       ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);    
        
        Photo photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId)
                      ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);
        
        if(photo.IsMain) throw new Exception("You cannot delete your main photo, please switch it before delete");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) throw new Exception("Cannot delete your photo");
        }

        user.Photos.Remove(photo);
        return await userRepository.SaveAllAsync();
    }
}
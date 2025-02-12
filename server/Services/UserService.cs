using AutoMapper;
using server.DTOs.Photo;
using server.DTOs.User;
using server.Entities;
using server.Enums;
using server.Exceptions;
using server.Helpers;
using server.Interfaces;
using Photo = server.Entities.Photo;

namespace server.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : IUserService
{
    public async Task<PagedList<UserResponseDto>> GetUsersAsync(UserParams userParams)
    {
        PagedList<AppUser> users = await unitOfWork.UserRepository.GetUsersAsync(userParams);
        PagedList<UserResponseDto> usersDto = new PagedList<UserResponseDto>([], users.TotalCount, users.CurrentPage, users.PageSize);
        
        mapper.Map(users, usersDto);
        return usersDto;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        AppUser? user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
        return mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> GetUserByUsernameAsync(string username)
    {
        AppUser? user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        return mapper.Map<UserResponseDto>(user);    
    }
    
    public async Task<bool> UpdateUser(string username, UserUpdateDto userDto)
    {
        AppUser user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username) 
                        ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);
        
        mapper.Map(userDto, user);
        return await unitOfWork.Complete();
    }
    
    public async Task<PhotoDto> AddPhoto(string username, IFormFile file)
    {
        AppUser user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username) 
                        ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) throw new Exception(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            IsMain = user.Photos.Count == 0
        };
        
        user.Photos.Add(photo);

        try
        {
            await unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add photo");
        }

        return mapper.Map<PhotoDto>(photo);
    }

    public async Task<bool> SetMainPhoto(string username, int photoId)
    {
        AppUser user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username) 
                       ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);

        Photo photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId)
                      ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);
        
        if (photo.IsMain) throw new Exception("This photo is already your main photo");

        Photo? currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        return await unitOfWork.Complete();
    }

    public async Task<bool> DeletePhoto(string username, int photoId)
    {
        AppUser user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username) 
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
        return await unitOfWork.Complete();
    }
}
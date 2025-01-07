using AutoMapper;
using server.DTOs.User;
using server.Entities;
using server.Enums;
using server.Exceptions;
using server.Interfaces;

namespace server.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
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

    public async Task<bool> UpdateUser(UserUpdateDto userDto, string username)
    {
        AppUser? user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) throw new ItemNotFoundException("Could not find ", EntityEnum.User);
        
        mapper.Map(userDto, user);
        return await userRepository.SaveAllAsync();
    }
}
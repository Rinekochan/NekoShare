﻿using AutoMapper;
using NekoShare.DTOs.User;
using NekoShare.Entities;
using NekoShare.Interfaces;

namespace NekoShare.Services;

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
}
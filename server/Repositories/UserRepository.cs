﻿using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Entities;
using server.Extensions;
using server.Helpers;
using server.Interfaces;

namespace server.Repositories;

public class UserRepository(DataContext context) : IUserRepository
{
    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    public async Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams)
    {
        var query = context.Users.Include(x => x.Photos).AsQueryable();

        query = query.Where(x => x.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears((-userParams.MaxAge - 1)));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears((-userParams.MinAge - 1)));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<AppUser>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username, string? currentUser)
    {
        var users = context.Users
            .Include(x => x.Photos)
            .AsQueryable();

        if (!string.IsNullOrEmpty(currentUser) && username.Equals(currentUser, StringComparison.OrdinalIgnoreCase))
        {
            await users.IgnoreQueryFilters().LoadAsync();
        }

        return await users
            .FirstOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<AppUser?> GetUserByPhotoId(int photoId)
    {
        return await context.Users
            .Include(x => x.Photos)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Photos.Any(p => p.Id == photoId));
        
    }
}
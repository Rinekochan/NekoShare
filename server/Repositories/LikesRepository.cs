using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs.User;
using server.Entities;
using server.Helpers;
using server.Interfaces;

namespace server.Repositories;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<UserResponseDto>> GetUserLikes(LikesParams likeParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<UserResponseDto> query;
        
        switch (likeParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceUserId == likeParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<UserResponseDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == likeParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<UserResponseDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likeParams.UserId);

                query = likes
                    .Where(x => x.TargetUserId == likeParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<UserResponseDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<UserResponseDto>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
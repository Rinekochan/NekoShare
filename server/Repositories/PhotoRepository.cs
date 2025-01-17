using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs.Photo;
using server.Entities;
using server.Enums;
using server.Exceptions;
using server.Interfaces;

namespace server.Repositories;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public Task<IQueryable<Photo>> GetUnapprovedPhotos()
    {
        return Task.FromResult(
            context.Photos
                .IgnoreQueryFilters()
                .Where(p => !p.IsApproved)
                .AsQueryable());
    }

    public async Task<Photo?> GetPhotoById(int photoId)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);
    }

    public async Task RemovePhoto(int photoId)
    {
        var photo = await GetPhotoById(photoId)
            ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);
        
       context.Photos.Remove(photo);
    }
}
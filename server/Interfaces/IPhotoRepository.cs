using server.DTOs.Photo;
using server.Entities;

namespace server.Interfaces;

public interface IPhotoRepository
{
    Task<IQueryable<Photo>> GetUnapprovedPhotos();
    Task<Photo?> GetPhotoById(int photoId);
    Task RemovePhoto(int photoId);
}
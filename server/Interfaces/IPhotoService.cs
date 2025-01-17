using CloudinaryDotNet.Actions;
using server.DTOs.Photo;

namespace server.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
    Task<IEnumerable<PhotoDto>> GetUnapprovedPhotos();
    Task<PhotoDto> GetPhotoById(int photoId);
    Task<bool> RemovePhotoByAdmin(int photoId);
    Task<bool> AprrovePhoto(int photoId);
    Task<bool> RejectPhoto(int photoId);
}
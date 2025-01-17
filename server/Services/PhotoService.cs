using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.DTOs.Photo;
using server.Enums;
using server.Exceptions;
using server.Helpers;
using server.Interfaces;

namespace server.Services;

public class PhotoService(IOptions<CloudinarySettings> config, IUnitOfWork unitOfWork, IMapper mapper) : IPhotoService
{
    private readonly Cloudinary _cloudinary = new Cloudinary(
        new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret)
    );

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "NekoShare"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }

    public async Task<IEnumerable<PhotoDto>> GetUnapprovedPhotos()
    {
        var photos = await unitOfWork.PhotoRepository
            .GetUnapprovedPhotos();

        return await photos
            .ProjectTo<PhotoDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PhotoDto> GetPhotoById(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId)
            ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);

        return mapper.Map<PhotoDto>(photo);
    }

    public async Task<bool> RemovePhotoByAdmin(int photoId)
    {
        await unitOfWork.PhotoRepository.RemovePhoto(photoId);

        return await unitOfWork.Complete();
    }

    public async Task<bool> AprrovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId)
                    ?? throw new ItemNotFoundException("Could not find ", EntityEnum.Photo);

        photo.IsApproved = true;

        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId)
                   ?? throw new ItemNotFoundException("Could not find ", EntityEnum.User);

        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;
        
        return await unitOfWork.Complete();
    }
    
    public async Task<bool> RejectPhoto(int photoId)
    {
        return await RemovePhotoByAdmin(photoId);
    }
}
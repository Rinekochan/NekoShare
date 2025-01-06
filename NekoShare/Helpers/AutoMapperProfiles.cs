using AutoMapper;
using NekoShare.DTOs.Photo;
using NekoShare.DTOs.User;
using NekoShare.Entities;

namespace NekoShare.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, UserResponseDto>()
            .ForMember(d => d.PhotoUrl, 
                o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
    }
}
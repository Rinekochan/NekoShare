using AutoMapper;
using server.DTOs.Photo;
using server.DTOs.User;
using server.Entities;

namespace server.Helpers;

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
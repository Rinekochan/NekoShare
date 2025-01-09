using AutoMapper;
using server.DTOs.Authenticate;
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
        CreateMap<UserUpdateDto, AppUser>();
        CreateMap<Photo, PhotoDto>();
        CreateMap<AuthenticateRequestDto, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
    }
}
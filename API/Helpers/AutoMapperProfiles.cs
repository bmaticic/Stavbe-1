using System;
using API.DTOs;
using API.Entities;
using API.Entities.MojElektro;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, o =>
                o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));

        CreateMap<Photo, PhotoDto>();
        CreateMap<PhotoStavbe, PhotoDto>();

        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));

        CreateMap<StavbaUpdateDto, Stavba>();
        CreateMap<Stavba, StavbaDto>()
            .ForMember(s => s.PhotoUrl, d =>
                 d.MapFrom(s => s.PhotosStavbe.FirstOrDefault(x => x.IsMain)!.Url));

        CreateMap<MerilnoMesto, MerilnoMestoDto>();
        CreateMap<GeoTocka, GeoTockaDto>();

        CreateMap<MojElektroMerilnoMesto, MojElektroMerilnoMestoDto>()
            .ForMember(d => d.PhotoUrl, o =>
                o.MapFrom(s => s.Stavba!.PhotosStavbe.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.IdJavnegaObjekta, o =>
                o.MapFrom(s => s.Stavba!.Id)); 
    }
}

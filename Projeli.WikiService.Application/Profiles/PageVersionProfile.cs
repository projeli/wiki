using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class PageVersionProfile : Profile
{
    public PageVersionProfile()
    {
        CreateMap<PageVersion, PageVersionDto>();
        CreateMap<PageVersionDto, PageVersion>();
    }
}
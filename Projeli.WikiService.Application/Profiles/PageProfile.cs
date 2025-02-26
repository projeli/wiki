using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<Page, PageDto>();
        CreateMap<PageDto, Page>();
    }
}
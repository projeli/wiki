using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<Page, PageDto>();
        CreateMap<PageDto, Page>();

        CreateMap<PageDto, SimplePageResponse>();
        CreateMap<PageDto, PageResponse>();

        CreateMap<CreatePageRequest, PageDto>();
        CreateMap<UpdatePageRequest, PageDto>();
        CreateMap<UpdatePageContentRequest, PageDto>();
    }
}
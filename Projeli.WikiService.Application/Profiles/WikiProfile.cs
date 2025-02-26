using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class WikiProfile : Profile
{
    public WikiProfile()
    {
        CreateMap<Wiki, WikiDto>();
        CreateMap<WikiDto, Wiki>();
    }
}
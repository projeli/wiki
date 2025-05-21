using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class WikiMemberProfile : Profile
{
    public WikiMemberProfile()
    {
        CreateMap<WikiMember, WikiMemberDto>();
        CreateMap<WikiMemberDto, WikiMember>();

        CreateMap<WikiMemberDto, WikiMemberResponse>();
        CreateMap<WikiMemberDto, SimpleWikiMemberResponse>();
    }
}
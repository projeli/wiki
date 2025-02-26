using AutoMapper;
using AutoMapper.Execution;
using Projeli.WikiService.Application.Dtos;

namespace Projeli.WikiService.Application.Profiles;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Member, MemberDto>();
        CreateMap<MemberDto, Member>();
    }
}
using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class WikiConfigProfile : Profile
{
    public WikiConfigProfile()
    {
        CreateMap<WikiConfig, WikiConfigDto>();
        CreateMap<WikiConfigDto, WikiConfig>();
        
        CreateMap<WikiConfig.WikiConfigSidebar, WikiConfigDto.WikiConfigSidebarDto>();
        CreateMap<WikiConfigDto.WikiConfigSidebarDto, WikiConfig.WikiConfigSidebar>();
        
        CreateMap<WikiConfig.WikiConfigSidebar.WikiConfigSidebarItem, WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto>();
        CreateMap<WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto, WikiConfig.WikiConfigSidebar.WikiConfigSidebarItem>();
    }
}
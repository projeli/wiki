using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class WikiStatisticsProfile : Profile
{
    public WikiStatisticsProfile()
    {
        CreateMap<WikiStatistics, WikiStatisticsDto>();
        CreateMap<WikiStatisticsDto, WikiStatistics>();
        
        CreateMap<WikiStatisticsDto, WikiStatisticsResponse>();
    }
}
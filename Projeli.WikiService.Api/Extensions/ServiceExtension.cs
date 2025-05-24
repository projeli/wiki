using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Extensions;

public static class ServiceExtension
{
    public static void AddWikiServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IWikiService, Application.Services.WikiService>();
        services.AddScoped<IWikiConfigService, Application.Services.WikiConfigService>();
        services.AddScoped<IWikiCategoryService, Application.Services.WikiCategoryService>();
        services.AddScoped<IWikiPageService, Application.Services.WikiPageService>();
        services.AddScoped<IWikiMemberService, Application.Services.WikiMemberService>();
        services.AddScoped<IWikiEventService, Application.Services.WikiEventService>();
    }
}
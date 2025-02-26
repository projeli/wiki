using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Extensions;

public static class ServiceExtension
{
    public static void AddWikiServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IWikiService, Application.Services.WikiService>();
    }
}
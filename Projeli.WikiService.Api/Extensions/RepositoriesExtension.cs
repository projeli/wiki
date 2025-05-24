using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Repositories;

namespace Projeli.WikiService.Api.Extensions;

public static class RepositoriesExtension
{
    public static void AddWikiServiceRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWikiRepository, WikiRepository>();
        services.AddScoped<IWikiConfigRepository, WikiConfigRepository>();
        services.AddScoped<IWikiCategoryRepository, WikiCategoryRepository>();
        services.AddScoped<IWikiPageRepository, WikiPageRepository>();
        services.AddScoped<IWikiMemberRepository, WikiMemberRepository>();
        services.AddScoped<IWikiEventRepository, WikiEventRepository>();
        services.AddScoped<IBusRepository, BusRepository>();
    }
}
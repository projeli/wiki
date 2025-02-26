using Projeli.WikiService.Api.Middlewares;

namespace Projeli.WikiService.Api.Extensions;

public static class MiddlewareExtension
{
    public static void UseWikiServiceMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<DatabaseExceptionMiddleware>();
        builder.UseMiddleware<HttpExceptionMiddleware>();
    }
}
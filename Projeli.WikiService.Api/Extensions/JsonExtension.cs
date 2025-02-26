using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projeli.WikiService.Api.Extensions;

public static class JsonExtension
{
    public static void AddWikiServiceJson(this IMvcBuilder services)
    {
        services.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }
}
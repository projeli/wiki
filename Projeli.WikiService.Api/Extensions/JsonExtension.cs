using System.Text.Json;
using System.Text.Json.Serialization;
using Projeli.WikiService.Domain.Models;
using Projeli.Shared.Application.Converters;

namespace Projeli.WikiService.Api.Extensions;

public static class JsonExtension
{
    public static void AddWikiServiceJson(this IMvcBuilder services)
    {
        services.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringNumberEnumConverter<WikiMemberPermissions>());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }
}
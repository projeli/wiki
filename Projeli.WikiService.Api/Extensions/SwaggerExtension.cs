using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Projeli.WikiService.Api.Extensions;

public static class SwaggerExtension
{
    public static void AddWikiServiceSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            OpenApiInfo info = new()
            {
                Title = "Wiki Service API",
                Version = "v1",
            };
            
            options.SwaggerDoc("v1", info);
            
            info.Version = "v2";
            options.SwaggerDoc("v2", info);
            
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter '{token}'",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };
    
            options.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            options.AddSecurityRequirement(securityRequirement);
            options.SchemaFilter<UlidSchemaFilter>();
        });
    }
    
    public static void UseWikiServiceSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
                options.SwaggerEndpoint($"/swagger/v2/swagger.json", "v2");
            }
        );
    }
    
    public class UlidSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Ulid))
            {
                schema.Type = "string";
                schema.Format = null; // Optional: You could set a custom format like "ulid" if desired
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString("01H3X9K7P5V8R2M4N6Q0T1J2B");
            }
        }
    }
}
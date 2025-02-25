using Microsoft.EntityFrameworkCore;
using WikiService.Api.Exceptions;
using WikiService.Infrastructure.Database;

namespace WikiService.Api.Extensions;

public static class DatabaseExtension
{
    public static void AddWikiServiceDatabase(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration["Database:ConnectionString"];
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new MissingEnvironmentVariableException("Database:ConnectionString");
        }

        services.AddDbContext<WikiServiceDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder => { builder.MigrationsAssembly("WikiService.Api"); });
            
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }

    public static void UseWikiServiceDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<WikiServiceDbContext>();
        if (database.Database.GetPendingMigrations().Any())
        {
            database.Database.Migrate();
        }
    }
}
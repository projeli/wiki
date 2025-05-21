using Microsoft.EntityFrameworkCore;
using Projeli.Shared.Infrastructure.Exceptions;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Api.Extensions;

public static class DatabaseExtension
{
    public static void AddWikiServiceDatabase(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var postgesConnectionString = configuration["Database:ConnectionString"];
        
        if (string.IsNullOrEmpty(postgesConnectionString))
        {
            throw new MissingEnvironmentVariableException("Database:ConnectionString");
        }

        services.AddDbContext<WikiServiceDbContext>(options =>
        {
            options.UseNpgsql(postgesConnectionString, builder => { builder.MigrationsAssembly("Projeli.WikiService.Api"); });
            
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
        
        var kurrentConnectionString = configuration["Kurrent:ConnectionString"];
        
        if (string.IsNullOrEmpty(kurrentConnectionString))
        {
            throw new MissingEnvironmentVariableException("Kurrent:ConnectionString");
        }

        services.AddKurrentDBClient(kurrentConnectionString);

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
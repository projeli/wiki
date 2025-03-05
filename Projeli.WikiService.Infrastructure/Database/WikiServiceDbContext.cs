using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Projeli.Shared.Infrastructure.Converters;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Infrastructure.Database;

public class WikiServiceDbContext(DbContextOptions<WikiServiceDbContext> options) : DbContext(options)
{
    public DbSet<Wiki> Wikis { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<WikiMember> Members { get; set; }
    public DbSet<PageVersion> PageVersions { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>()
            .HaveConversion<UlidToGuidConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var memberInfo = property.PropertyInfo ?? (MemberInfo?)property.FieldInfo;
                if (memberInfo == null) continue;
                var defaultValue =
                    Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                if (defaultValue == null) continue;
                property.SetDefaultValue(defaultValue.Value);
            }
        }

        builder.ApplyConfigurationsFromAssembly(typeof(WikiServiceDbContext).Assembly);

        builder.Entity<Wiki>()
            .OwnsOne<WikiConfig>(x => x.Config,
                y =>
                {
                    y.ToJson();
                    y.OwnsOne(z => z.Sidebar, a =>
                    {
                        a.OwnsMany(b => b.Items, c =>
                        {
                            c.Property(x => x.Title).IsRequired();
                            c.Property(x => x.Href).IsRequired();
                            c.OwnsMany(x => x.Category, x =>
                            {
                                x.Property(x => x.Title).IsRequired();
                                x.Property(x => x.Href).IsRequired();
                            });
                        });
                    });
                });

        builder.Entity<Page>()
            .HasOne(x => x.Wiki)
            .WithMany(x => x.Pages)
            .HasForeignKey(x => x.WikiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Page>()
            .HasMany(x => x.Categories)
            .WithMany(x => x.Pages);

        builder.Entity<Page>()
            .HasMany(x => x.Editors)
            .WithMany(x => x.Pages);

        builder.Entity<Category>()
            .HasOne(x => x.Wiki)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.WikiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Category>()
            .HasMany(x => x.Pages)
            .WithMany(x => x.Categories);

        builder.Entity<WikiMember>()
            .HasOne(x => x.Wiki)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.WikiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PageVersion>()
            .HasOne(x => x.Page)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PageVersion>()
            .HasMany(x => x.Editors)
            .WithMany(x => x.PageVersions);
    }
}
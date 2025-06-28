using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiCategoryRepository(WikiServiceDbContext database) : IWikiCategoryRepository
{
    public Task<List<Category>> GetByWikiId(Ulid wikiId, string? userId)
    {
        return database.Wikis
            .Include(x => x.Categories)
            .Include(x => x.Members)
            .Where(x => x.Id == wikiId &&
                        (x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Categories)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task<List<Category>> GetByProjectId(Ulid projectId, string? userId)
    {
        return database.Wikis
            .Include(x => x.Categories)
            .Include(x => x.Members)
            .Where(x => x.ProjectId == projectId &&
                        (x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Categories)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task<List<Category>> GetByProjectSlug(string wikiId, string? userId)
    {
        return database.Wikis
            .Include(x => x.Categories)
            .Include(x => x.Members)
            .Where(x => x.ProjectSlug == wikiId &&
                        (x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Categories)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task<Category?> GetById(Ulid wikiId, Ulid categoryId, string? userId = null, bool force = false)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.WikiId == wikiId &&
                        x.Id == categoryId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public Task<Category?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.WikiId == wikiId &&
                        x.Slug == slug &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public Task<Category?> GetByProjectIdAndId(Ulid projectId, Ulid categoryId, string? userId)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectId == projectId &&
                        x.Id == categoryId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public Task<Category?> GetByProjectIdAndSlug(Ulid projectId, string categorySlug, string? userId)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectId == projectId &&
                        x.Slug == categorySlug &&
                        (x.Wiki.Status == WikiStatus.Published  ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public Task<Category?> GetByProjectSlugAndId(string projectSlug, Ulid categoryId, string? userId)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectSlug == projectSlug &&
                        x.Id == categoryId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public Task<Category?> GetByProjectSlugAndSlug(string projectSlug, string categorySlug, string? userId)
    {
        return database.Categories
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectSlug == projectSlug &&
                        x.Slug == categorySlug &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectCategoryWithPages)
            .FirstOrDefaultAsync();
    }

    public async Task<Category?> Create(Ulid wikiId, Category category)
    {
        var wiki = await database.Wikis
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == wikiId);

        if (wiki is null) return null;

        wiki.Categories.Add(category);
        await database.SaveChangesAsync();

        return category;
    }

    public async Task<Category?> Update(Ulid wikiId, Category category)
    {
        var existingCategory = await database.Categories
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == category.Id);

        if (existingCategory is null) return null;

        existingCategory.Name = category.Name;
        existingCategory.Slug = category.Slug;
        existingCategory.Description = category.Description;
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingCategory;
    }

    public async Task<Category?> UpdatePages(Ulid wikiId, Category category, List<Ulid> pageIds)
    {
        var existingCategory = await database.Categories
            .Include(x => x.Pages)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == category.Id);

        if (existingCategory == null) return null;

        existingCategory.Pages.Clear();

        var existingPages = await database.Pages
            .Where(x => x.WikiId == wikiId && pageIds.Contains(x.Id))
            .ToListAsync();

        existingCategory.Pages.AddRange(existingPages);

        await database.SaveChangesAsync();

        return existingCategory;
    }

    public async Task<bool> Delete(Ulid wikiId, Ulid categoryId)
    {
        var category = await database.Categories
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == categoryId);

        if (category is null) return false;

        database.Categories.Remove(category);
        await database.SaveChangesAsync();

        return true;
    }
    
    private static readonly Expression<Func<Category, Category>> SelectCategoryWithPages = x => new Category
    {
        Id = x.Id,
        WikiId = x.WikiId,
        Name = x.Name,
        Slug = x.Slug,
        Description = x.Description,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt,
        Pages = x.Pages
            .OrderBy(y => y.Title)
            .Select(y => new Page
            {
                Id = y.Id,
                Title = y.Title,
                Slug = y.Slug
            }).ToList()
    };
}
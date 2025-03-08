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
        return database.Wikis
            .Include(x => x.Categories)
            .Include(x => x.Members)
            .Where(x => x.Id == wikiId &&
                        (force || x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == categoryId);
    }

    public Task<Category?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false)
    {
        return database.Wikis
            .Include(x => x.Categories)
            .Include(x => x.Members)
            .Where(x => x.Id == wikiId &&
                        (force || x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Slug == slug);
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

    public async Task<bool> Delete(Ulid wikiId, Ulid categoryId)
    {
        var category = await database.Categories
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == categoryId);

        if (category is null) return false;

        database.Categories.Remove(category);
        await database.SaveChangesAsync();

        return true;
    }
}
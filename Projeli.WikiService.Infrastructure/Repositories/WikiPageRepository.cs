using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiPageRepository(WikiServiceDbContext database) : IWikiPageRepository
{
    public Task<List<Page>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.WikiId == wikiId &&
                        (force || x.Wiki.Status == WikiStatus.Published ||
                         x.Wiki.Members.Any(y => y.UserId == userId)))
            .Select(SelectSimplePage)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public Task<List<Page>> GetByProjectId(Ulid projectId, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectId == projectId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectSimplePage)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public Task<List<Page>> GetByProjectSlug(string wikiId, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectSlug == wikiId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectSimplePage)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public Task<Page?> GetById(Ulid wikiId, Ulid pageId, string? userId = null, bool force = false)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.WikiId == wikiId &&
                        x.Id == pageId &&
                        (force || x.Wiki.Status == WikiStatus.Published ||
                         x.Wiki.Members.Any(y => y.UserId == userId)))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public Task<Page?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.WikiId == wikiId &&
                        x.Slug == slug &&
                        (force || x.Wiki.Status == WikiStatus.Published ||
                         x.Wiki.Members.Any(y => y.UserId == userId)))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public Task<Page?> GetByProjectIdAndId(Ulid projectId, Ulid pageId, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectId == projectId &&
                        x.Id == pageId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public Task<Page?> GetByProjectIdAndSlug(Ulid projectId, string pageSlug, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectId == projectId &&
                        x.Slug == pageSlug &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public Task<Page?> GetByProjectSlugAndId(string projectSlug, Ulid pageId, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectSlug == projectSlug &&
                        x.Id == pageId &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public Task<Page?> GetByProjectSlugAndSlug(string projectSlug, string pageSlug, string? userId)
    {
        return database.Pages
            .AsNoTracking()
            .Where(x => x.Wiki.ProjectSlug == projectSlug &&
                        x.Slug == pageSlug &&
                        (x.Wiki.Status == WikiStatus.Published ||
                         (userId != null && x.Wiki.Members.Any(y => y.UserId == userId))))
            .Select(SelectPageWithCategories)
            .FirstOrDefaultAsync();
    }

    public async Task<Page?> Create(Ulid wikiId, Page page)
    {
        var wiki = await database.Wikis
            .Include(x => x.Pages)
            .FirstOrDefaultAsync(x => x.Id == wikiId);

        if (wiki == null) return null;

        wiki.Pages.Add(page);
        await database.SaveChangesAsync();

        return page;
    }

    public async Task<Page?> Update(Ulid wikiId, Page page)
    {
        var existingPage = await database.Pages
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == page.Id);

        if (existingPage == null) return null;

        existingPage.Title = page.Title;
        existingPage.Slug = page.Slug;
        existingPage.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingPage;
    }

    public async Task<Page?> UpdateContent(Ulid wikiId, Page page)
    {
        var existingPage = await database.Pages
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == page.Id);

        if (existingPage == null) return null;

        existingPage.Content = page.Content;
        existingPage.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingPage;
    }

    public async Task<Page?> UpdateCategories(Ulid wikiId, Page page, List<Ulid> categoryIds)
    {
        var existingPage = await database.Pages
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == page.Id);

        if (existingPage == null) return null;

        existingPage.Categories.Clear();
        
        var existingCategories = await database.Categories
            .Where(x => x.WikiId == wikiId && categoryIds.Contains(x.Id))
            .ToListAsync();
        
        existingPage.Categories.AddRange(existingCategories);
        
        await database.SaveChangesAsync();
        
        return existingPage;
    }

    public async Task<Page?> UpdateStatus(Ulid wikiId, Ulid pageId, PageStatus status)
    {
        var page = await database.Pages
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == pageId);

        if (page == null) return null;

        page.Status = status;
        page.PublishedAt ??= status == PageStatus.Published ? DateTime.UtcNow : null;

        await database.SaveChangesAsync();

        return page;
    }

    public async Task<bool> Delete(Ulid wikiId, Ulid pageId)
    {
        var page = await database.Pages
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == pageId);

        if (page == null) return false;

        database.Pages.Remove(page);
        await database.SaveChangesAsync();

        return true;
    }

    private static readonly Expression<Func<Page, Page>> SelectSimplePage = x => new Page
    {
        Id = x.Id,
        Title = x.Title,
        Slug = x.Slug,
        Status = x.Status
    };

    private static readonly Expression<Func<Page, Page>> SelectPageWithCategories = x => new Page
    {
        Id = x.Id,
        WikiId = x.WikiId,
        Title = x.Title,
        Slug = x.Slug,
        Content = x.Content,
        Status = x.Status,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt,
        PublishedAt = x.PublishedAt,
        Categories = x.Categories
            .OrderBy(y => y.Name)
            .Select(y => new Category
            {
                Id = y.Id,
                Name = y.Name,
                Slug = y.Slug
            }).ToList()
    };
    // hoiiiiiii ik ben nu aan het programmeren, code taal blablabla 
}
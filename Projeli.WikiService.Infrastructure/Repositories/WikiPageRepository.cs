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
        return database.Wikis
            .Include(x => x.Pages)
            .Include(x => x.Members)
            .Where(x => x.Id == wikiId &&
                        (force || x.Status == WikiStatus.Published || x.Members.Any(y => y.UserId == userId)))
            .SelectMany(x => x.Pages)
            .ToListAsync();
    }

    public Task<Page?> GetById(Ulid wikiId, Ulid pageId, string? userId = null, bool force = false)
    {
        return database.Pages
            .Include(x => x.Wiki)
            .Include(x => x.Wiki.Members)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId &&
                                      x.Id == pageId &&
                                      (force || x.Wiki.Status == WikiStatus.Published ||
                                       x.Wiki.Members.Any(y => y.UserId == userId)));
    }

    public Task<Page?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false)
    {
        return database.Pages
            .Include(x => x.Wiki)
            .Include(x => x.Wiki.Members)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId &&
                                      x.Slug == slug &&
                                      (force || x.Wiki.Status == WikiStatus.Published ||
                                       x.Wiki.Members.Any(y => y.UserId == userId)));
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
            .Include(x => x.Wiki)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == page.Id);

        if (existingPage == null) return null;

        existingPage.Title = page.Title;
        existingPage.Slug = page.Slug;
        existingPage.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingPage;
    }

    public async Task<bool> Delete(Ulid wikiId, Ulid pageId)
    {
        var page = await database.Pages
            .Include(x => x.Wiki)
            .FirstOrDefaultAsync(x => x.WikiId == wikiId && x.Id == pageId);

        if (page == null) return false;

        database.Pages.Remove(page);
        await database.SaveChangesAsync();

        return true;
    }
}
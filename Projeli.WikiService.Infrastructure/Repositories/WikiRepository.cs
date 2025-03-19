using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Infrastructure.Database;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiRepository(WikiServiceDbContext database) : IWikiRepository
{
    public Task<Wiki?> GetById(Ulid id, string? userId, bool force = false)
    {
        return database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.Id == id)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.Status == WikiStatus.Published || wiki.Members.Any(member => member.UserId == userId));
    }

    public Task<Wiki?> GetByProjectId(Ulid projectId, string? userId, bool force = false)
    {
        return database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.ProjectId == projectId)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.Status == WikiStatus.Published || wiki.Members.Any(member => member.UserId == userId));
    }

    public Task<Wiki?> GetByProjectSlug(string projectSlug, string? userId, bool force = false)
    {
        return database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.ProjectSlug == projectSlug)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.Status == WikiStatus.Published || wiki.Members.Any(member => member.UserId == userId));
    }

    public async Task<WikiStatistics?> GetStatistics(Ulid id, string? userId)
    {
        return await database.Wikis
            .AsNoTracking()
            .Where(wiki =>
                wiki.Id == id && (wiki.Status == WikiStatus.Published ||
                                  wiki.Members.Any(member => member.UserId == userId)))
            .Select(wiki => new WikiStatistics
            {
                WikiId = wiki.Id,
                PageCount = wiki.Pages.Count,
                MemberCount = wiki.Members.Count,
                CategoryCount = wiki.Categories.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Wiki?> Create(Wiki wiki)
    {
        var createdWiki = await database.Wikis.AddAsync(wiki);
        await database.SaveChangesAsync();

        return createdWiki.Entity;
    }

    public async Task<Wiki?> Update(Wiki wiki)
    {
        var existingWiki = await database.Wikis
            .Include(w => w.Members)
            .FirstOrDefaultAsync(w => w.Id == wiki.Id);
        if (existingWiki is null) return null;

        // Get the list of members from the DTO
        var updatedMemberIds = wiki.Members.Select(m => m.Id).ToList();

        // Remove members that are no longer in the DTO
        existingWiki.Members.RemoveAll(m => !updatedMemberIds.Contains(m.Id));

        // Update or create new members
        foreach (var member in wiki.Members)
        {
            var existingMember = existingWiki.Members.FirstOrDefault(m => m.Id == member.Id);
            if (existingMember is not null)
            {
                // Update existing member
                existingMember.IsOwner = member.IsOwner;
                existingMember.Permissions = member.Permissions;
            }
            else
            {
                // Create new member
                member.Id = Ulid.NewUlid();
                member.WikiId = wiki.Id;
                existingWiki.Members.Add(member);
            }
        }

        // Update the rest of the properties
        existingWiki.ProjectId = wiki.ProjectId;
        existingWiki.ProjectName = wiki.ProjectName;
        existingWiki.ProjectSlug = wiki.ProjectSlug;
        existingWiki.Name = wiki.Name;
        existingWiki.Content = wiki.Content;
        existingWiki.Config = wiki.Config;
        existingWiki.UpdatedAt = wiki.UpdatedAt;
        existingWiki.Status = wiki.Status;

        await database.SaveChangesAsync();

        return existingWiki;
    }

    public async Task<Wiki?> UpdateStatus(Ulid id, WikiStatus status)
    {
        var existingWiki = await database.Wikis.FirstOrDefaultAsync(w => w.Id == id);
        if (existingWiki is null) return null;

        existingWiki.Status = status;
        await database.SaveChangesAsync();

        return existingWiki;
    }

    public async Task<Wiki?> UpdateContent(Ulid id, string content)
    {
        var existingWiki = await database.Wikis.FirstOrDefaultAsync(w => w.Id == id);
        if (existingWiki is null) return null;

        existingWiki.Content = content;
        existingWiki.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingWiki;
    }

    public async Task<Wiki?> UpdateSidebar(Ulid id, WikiConfig.WikiConfigSidebar sidebar)
    {
        var existingWiki = await database.Wikis.FirstOrDefaultAsync(w => w.Id == id);
        if (existingWiki is null) return null;

        existingWiki.Config.Sidebar = sidebar;
        existingWiki.UpdatedAt = DateTime.UtcNow;

        await database.SaveChangesAsync();

        return existingWiki;
    }

    public async Task<bool> Delete(Ulid id)
    {
        var existingWiki = await database.Wikis.FirstOrDefaultAsync(w => w.Id == id);
        if (existingWiki is null) return false;

        database.Wikis.Remove(existingWiki);
        await database.SaveChangesAsync();

        return true;
    }
}
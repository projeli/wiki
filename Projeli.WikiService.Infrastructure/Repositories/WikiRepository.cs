using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Infrastructure.Database;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiRepository(WikiServiceDbContext database) : IWikiRepository
{
    public async Task<Wiki?> GetById(Ulid id, string? userId, bool force = false)
    {
        return await database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.Id == id)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.IsPublished || wiki.Members.Any(member => member.UserId == userId));
    }

    public async Task<Wiki?> GetByProjectId(Ulid projectId, string? userId, bool force = false)
    {
        return await database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.ProjectId == projectId)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.IsPublished || wiki.Members.Any(member => member.UserId == userId));
    }

    public async Task<Wiki?> GetByProjectSlug(string projectSlug, string? userId, bool force = false)
    {
        return await database.Wikis
            .AsNoTracking()
            .Include(wiki => wiki.Members)
            .Where(wiki => wiki.ProjectSlug == projectSlug)
            .FirstOrDefaultAsync(wiki =>
                force || wiki.IsPublished || wiki.Members.Any(member => member.UserId == userId));
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
        existingWiki.UpdatedAt = wiki.UpdatedAt;
        
        await database.SaveChangesAsync();
        
        return existingWiki;
    }
}
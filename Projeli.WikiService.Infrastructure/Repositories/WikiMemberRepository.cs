using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiMemberRepository(WikiServiceDbContext database) : IWikiMemberRepository
{
    public async Task<WikiMember?> Get(Ulid wikiId, string userId)
    {
        var wikiMember = await database.Members
            .FirstOrDefaultAsync(member => member.WikiId == wikiId && member.UserId == userId);

        return wikiMember;
    }

    public async Task<WikiMember?> Add(Ulid wikiId, WikiMember member)
    {
        var existingMember = await database.Members
            .FirstOrDefaultAsync(m => m.WikiId == wikiId && m.UserId == member.UserId);

        if (existingMember is not null) return null;

        member.WikiId = wikiId;
        await database.Members.AddAsync(member);
        await database.SaveChangesAsync();

        return member;
    }

    public async Task<WikiMember?> UpdatePermissions(Ulid wikiId, Ulid userId, WikiMemberPermissions permissions)
    {
        var wikiMember = await database.Members
            .FirstOrDefaultAsync(member => member.WikiId == wikiId && member.Id == userId);

        if (wikiMember is null) return null;

        wikiMember.Permissions = permissions;
        await database.SaveChangesAsync();

        return wikiMember;
    }

    public async Task<bool> Delete(Ulid wikiId, string userId)
    {
        var wikiMember = await database.Members
            .FirstOrDefaultAsync(member => member.WikiId == wikiId && member.UserId == userId);

        if (wikiMember is null) return false;

        database.Members.Remove(wikiMember);
        
        return await database.SaveChangesAsync() > 0;
    }
}
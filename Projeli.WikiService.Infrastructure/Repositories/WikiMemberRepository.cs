using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiMemberRepository(WikiServiceDbContext database) : IWikiMemberRepository
{
    public async Task<WikiMember?> UpdatePermissions(Ulid wikiId, Ulid userId, WikiMemberPermissions permissions)
    {
        var wikiMember = await database.Members
            .FirstOrDefaultAsync(member => member.WikiId == wikiId && member.Id == userId);

        if (wikiMember is null) return null;

        wikiMember.Permissions = permissions;
        await database.SaveChangesAsync();

        return wikiMember;
    }
}
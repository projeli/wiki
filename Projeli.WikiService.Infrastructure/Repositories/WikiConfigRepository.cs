using Microsoft.EntityFrameworkCore;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;
using Projeli.WikiService.Infrastructure.Database;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiConfigRepository(WikiServiceDbContext database) : IWikiConfigRepository
{
    public async Task<WikiConfig?> GetByWikiId(Ulid wikiId, string? userId)
    {
        var wiki = await database.Wikis
            .AsNoTracking()
            .Include(w => w.Members)
            .Where(w => w.Status == WikiStatus.Published || w.Members.Any(m => m.UserId == userId))
            .Select(w => new Wiki
            {
                Id = w.Id,
                Config = w.Config
            })
            .FirstOrDefaultAsync(w => w.Id == wikiId);
        
        return wiki?.Config;
    }
}
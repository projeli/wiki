using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiConfigRepository
{
    Task<WikiConfig?> GetByWikiId(Ulid wikiId, string? userId);
}
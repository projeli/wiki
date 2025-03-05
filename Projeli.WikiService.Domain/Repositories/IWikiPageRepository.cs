using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiPageRepository
{
    Task<List<Page>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false);
    Task<Page?> GetById(Ulid wikiId, Ulid pageId, string? userId = null, bool force = false);
    Task<Page?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false);
    Task<Page?> Create(Ulid wikiId, Page page);
    Task<Page?> Update(Ulid wikiId, Page page);
    Task<bool> Delete(Ulid wikiId, Ulid pageId);
}
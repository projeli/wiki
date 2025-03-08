using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiPageRepository
{
    Task<List<Page>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false);
    Task<List<Page>> GetByProjectId(Ulid projectId, string? userId);
    Task<List<Page>> GetByProjectSlug(string wikiId, string? userId);
    Task<Page?> GetById(Ulid wikiId, Ulid pageId, string? userId = null, bool force = false);
    Task<Page?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false);
    Task<Page?> GetByProjectIdAndId(Ulid projectId, Ulid pageId, string? userId);
    Task<Page?> GetByProjectIdAndSlug(Ulid projectId, string pageSlug, string? userId);
    Task<Page?> GetByProjectSlugAndId(string projectSlug, Ulid pageId, string? userId);
    Task<Page?> GetByProjectSlugAndSlug(string projectSlug, string pageSlug, string? userId);
    Task<Page?> Create(Ulid wikiId, Page page);
    Task<Page?> Update(Ulid wikiId, Page page);
    Task<Page?> UpdateContent(Ulid wikiId, Page page);
    Task<Page?> UpdateCategories(Ulid wikiId, Page page, List<Ulid> categoryIds);
    Task<Page?> UpdateStatus(Ulid wikiId, Ulid pageId, PageStatus status);
    Task<bool> Delete(Ulid wikiId, Ulid pageId);
}
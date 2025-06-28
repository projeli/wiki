using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiCategoryRepository
{
    Task<List<Category>> GetByWikiId(Ulid wikiId, string? userId);
    Task<List<Category>> GetByProjectId(Ulid projectId, string? userId);
    Task<List<Category>> GetByProjectSlug(string wikiId, string? userId);
    Task<Category?> GetById(Ulid wikiId, Ulid categoryId, string? userId = null, bool force = false);
    Task<Category?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false);
    Task<Category?> GetByProjectIdAndId(Ulid projectId, Ulid categoryId, string? userId);
    Task<Category?> GetByProjectIdAndSlug(Ulid projectId, string categorySlug, string? userId);
    Task<Category?> GetByProjectSlugAndId(string projectSlug, Ulid categoryId, string? userId);
    Task<Category?> GetByProjectSlugAndSlug(string projectSlug, string categorySlug, string? userId);
    Task<Category?> Create(Ulid wikiId, Category category);
    Task<Category?> Update(Ulid wikiId, Category category);
    Task<Category?> UpdatePages(Ulid wikiId, Category category, List<Ulid> pageIds);
    Task<bool> Delete(Ulid wikiId, Ulid categoryId);
}
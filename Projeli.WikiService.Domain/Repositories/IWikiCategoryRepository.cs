using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiCategoryRepository
{
    Task<List<Category>> GetByWikiId(Ulid wikiId, string? userId);
    Task<Category?> GetById(Ulid wikiId, Ulid categoryId, string? userId = null, bool force = false);
    Task<Category?> GetBySlug(Ulid wikiId, string slug, string? userId = null, bool force = false);
    Task<Category?> Create(Ulid wikiId, Category category);
    Task<Category?> Update(Ulid wikiId, Category category);
    Task<bool> Delete(Ulid wikiId, Ulid categoryId);
}
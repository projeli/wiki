using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiCategoryService
{
    Task<IResult<List<CategoryDto>>> GetByWikiId(Ulid wikiId, string? userId);
    Task<IResult<CategoryDto?>> Create(Ulid wikiId, CategoryDto categoryDto, string userId);
    Task<IResult<CategoryDto?>> Update(Ulid wikiId, Ulid categoryId, CategoryDto categoryDto, string userId);
    Task<IResult<CategoryDto?>> Delete(Ulid wikiId, Ulid categoryId, string userId);
}
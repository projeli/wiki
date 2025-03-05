using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiPageService
{
    Task<IResult<List<PageDto>>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false);
    Task<IResult<PageDto?>> Create(Ulid wikiId, PageDto pageDto, string userId);
    Task<IResult<PageDto?>> Update(Ulid wikiId, Ulid pageId, PageDto pageDto, string userId);
    Task<IResult<PageDto?>> Delete(Ulid wikiId, Ulid pageId, string userId);
}
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiPageService
{
    Task<IResult<List<PageDto>>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false);
    Task<IResult<List<PageDto>>> GetByProjectId(Ulid projectId, string? userId);
    Task<IResult<List<PageDto>>> GetByProjectSlug(string wikiId, string? userId);
    Task<IResult<PageDto?>> GetById(Ulid wikiId, Ulid pageId, string? userId);
    Task<IResult<PageDto?>> GetBySlug(Ulid wikiId, string pageSlug, string? userId);
    Task<IResult<PageDto?>> GetByProjectIdAndId(Ulid projectId, Ulid pageId, string? userId);
    Task<IResult<PageDto?>> GetByProjectIdAndSlug(Ulid projectId, string pageSlug, string? userId);
    Task<IResult<PageDto?>> GetByProjectSlugAndId(string projectSlug, Ulid pageId, string? userId);
    Task<IResult<PageDto?>> GetByProjectSlugAndSlug(string projectSlug, string pageSlug, string? userId);
    Task<IResult<PageDto?>> Create(Ulid wikiId, PageDto pageDto, string userId);
    Task<IResult<PageDto?>> Update(Ulid wikiId, Ulid pageId, PageDto pageDto, string userId);
    Task<IResult<PageDto?>> UpdateContent(Ulid wikiId, Ulid pageId, string content, string userId);
    Task<IResult<PageDto?>> UpdateCategories(Ulid wikiId, Ulid pageId, List<Ulid> categoryIds, string userId);
    Task<IResult<PageDto?>> UpdateStatus(Ulid wikiId, Ulid pageId, PageStatus status, string userId);
    Task<IResult<PageDto?>> Delete(Ulid wikiId, Ulid pageId, string userId);
}
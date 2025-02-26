using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiService
{
    Task<IResult<WikiDto?>> GetById(Ulid id, string? userId, bool force = false);
    Task<IResult<WikiDto?>> GetByProjectId(Ulid projectId, string? userId, bool force = false);
    Task<IResult<WikiDto?>> GetByProjectSlug(string projectSlug, string? userId, bool force = false);
    Task<IResult<WikiDto?>> Create(WikiDto wikiDto);
    Task<IResult<WikiDto?>> Update(Ulid id, WikiDto wikiDto, string? userId, bool force = false);
}
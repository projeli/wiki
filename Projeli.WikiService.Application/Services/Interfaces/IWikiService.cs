using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiService
{
    Task<IResult<List<WikiDto>>> GetByIds(List<Ulid> ids, string? userId);
    Task<IResult<WikiDto?>> GetById(Ulid id, string? userId, bool force = false);
    Task<IResult<WikiDto?>> GetByProjectId(Ulid projectId, string? userId, bool force = false);
    Task<IResult<WikiDto?>> GetByProjectSlug(string projectSlug, string? userId, bool force = false);
    Task<IResult<WikiStatisticsDto?>> GetStatistics(Ulid id, string? userId);
    Task<IResult<WikiDto?>> Create(Ulid projectId, string projectName, string projectSlug, string? projectImageUrl, List<WikiMemberDto> members, string userId);
    Task<IResult<WikiDto?>> UpdateProjectDetails(Ulid id, WikiDto wikiDto);
    Task<IResult<WikiDto?>> UpdateStatus(Ulid id, WikiStatus status, string userId);
    Task<IResult<WikiDto?>> UpdateContent(Ulid id, string content, string userId);
    Task<IResult<WikiDto?>> UpdateSidebar(Ulid id, WikiConfigDto.WikiConfigSidebarDto sidebar, string userId);
    Task<IResult<WikiDto?>> UpdateOwnership(Ulid wikiId, string fromUserId, string toUserId, bool force = false);
    Task<IResult<WikiDto?>> Delete(Ulid id, string? userId, bool force = false);
}
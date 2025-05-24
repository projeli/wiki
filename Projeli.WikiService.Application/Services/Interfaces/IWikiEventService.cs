using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models.Events;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiEventService
{
    Task<PagedResult<BaseWikiEvent>> GetEvents(Ulid wikiId, List<string> userIds, List<string> eventTypes,
        int page, int pageSize, string? performingUserId, bool force = false);
}
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models.Events;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiEventRepository
{
    Task<PagedResult<BaseWikiEvent>> GetEvents(
        Ulid wikiId,
        List<string> userIds,
        List<string> eventTypes,
        int page,
        int pageSize,
        bool isForward = false
    );

    Task StoreEvent<T>(Ulid wikiId, T @event) where T : BaseWikiEvent;
    Task<bool> DeleteEvents(Ulid wikiId);
}
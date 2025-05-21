using Projeli.WikiService.Domain.Models.Events;

namespace Projeli.WikiService.Domain.Repositories;

public interface IEventRepository
{
    Task StoreEvent<T>(Ulid wikiId, T @event) where T : BaseWikiEvent;
    Task<List<BaseWikiEvent>> GetEvents(Ulid wikiId);
    Task<bool> DeleteEvents(Ulid wikiId);
}
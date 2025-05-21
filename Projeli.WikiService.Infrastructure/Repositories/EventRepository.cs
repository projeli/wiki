using System.Reflection;
using System.Text;
using System.Text.Json;
using KurrentDB.Client;
using Projeli.WikiService.Domain.Models.Events;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class EventRepository(KurrentDBClient kurrentDbClient) : IEventRepository
{
    public async Task StoreEvent<T>(Ulid wikiId, T @event) where T : BaseWikiEvent
    {
        var data = JsonSerializer.Serialize(@event);
        Console.WriteLine(data);
        await kurrentDbClient.AppendToStreamAsync(
            $"wiki-{wikiId}",
            StreamState.Any,
            [
                new EventData(
                    Uuid.FromGuid(Ulid.NewUlid().ToGuid()),
                    @event.GetType().Name,
                    Encoding.UTF8.GetBytes(data)
                )
            ]
        );
    }

    public async Task<List<BaseWikiEvent>> GetEvents(Ulid wikiId)
    {
        var result = kurrentDbClient.ReadStreamAsync(
            Direction.Backwards,
            $"wiki-{wikiId}",
            StreamPosition.End
        );

        var events = new List<BaseWikiEvent>();
        await foreach (var streamMessage in result.Messages)
        {
            if (streamMessage is StreamMessage.Event eventMessage)
            {
                var type = Assembly.GetAssembly(typeof(BaseWikiEvent))!.GetTypes()
                    .FirstOrDefault(t => t.Name == eventMessage.ResolvedEvent.Event.EventType);
                if (type is not null)
                {
                    var eventData = Encoding.UTF8.GetString(eventMessage.ResolvedEvent.Event.Data.ToArray());
                    var output = JsonSerializer.Deserialize(eventData, type);
                    if (output is BaseWikiEvent wikiEvent)
                    {
                        wikiEvent.Timestamp = eventMessage.ResolvedEvent.Event.Created;
                        events.Add(wikiEvent);
                    }
                }
            }
        }

        return events;
    }
    
    public async Task<bool> DeleteEvents(Ulid wikiId)
    {
        var result = await kurrentDbClient.DeleteAsync($"wiki-{wikiId}", StreamState.Any);
        return result.LogPosition == Position.Start;
    }
}
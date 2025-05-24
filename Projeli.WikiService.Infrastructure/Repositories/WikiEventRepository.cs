using System.Reflection;
using System.Text;
using System.Text.Json;
using KurrentDB.Client;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Domain.Models.Events;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Infrastructure.Repositories;

public class WikiEventRepository(KurrentDBClient kurrentDbClient) : IWikiEventRepository
{
    private static readonly Dictionary<string, Type> EventTypes =
        Assembly.GetAssembly(typeof(BaseWikiEvent))!
            .GetTypes()
            .Where(t => typeof(BaseWikiEvent).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);

    public async Task<PagedResult<BaseWikiEvent>> GetEvents(
        Ulid wikiId,
        List<string> userIds,
        List<string> eventTypes,
        int page,
        int pageSize,
        bool isForward = false)
    {
        // Validate inputs
        if (page < 1 || pageSize < 1)
            throw new ArgumentException("Page and pageSize must be positive integers.");

        // Prepare stream and reading parameters
        var streamName = $"wiki-{wikiId}";
        var startPosition = isForward ? StreamPosition.Start : StreamPosition.End;
        var result = kurrentDbClient.ReadStreamAsync(isForward ? Direction.Forwards : Direction.Backwards, streamName,
            startPosition);

        // Use HashSet for O(1) lookups; null if no filtering required
        var eventTypeSet = eventTypes.Count > 0 ? new HashSet<string>(eventTypes) : null;
        var userIdSet = userIds.Count > 0 ? new HashSet<string>(userIds) : null;

        // Pagination parameters
        var skip = (page - 1) * pageSize;
        var take = pageSize;
        var events = new List<BaseWikiEvent>();
        var skipped = 0;

        // Process stream events asynchronously
        await foreach (var streamMessage in result.Messages)
        {
            if (streamMessage is not StreamMessage.Event eventMessage)
                continue;

            var eventType = eventMessage.ResolvedEvent.Event.EventType;

            // Filter by event type if specified
            if (eventTypeSet != null && !eventTypeSet.Contains(eventType))
                continue;

            // Deserialize event data if it matches filters
            if (EventTypes.TryGetValue(eventType, out var type))
            {
                var eventData = Encoding.UTF8.GetString(eventMessage.ResolvedEvent.Event.Data.ToArray());
                var output = JsonSerializer.Deserialize(eventData, type);
                if (output is BaseWikiEvent wikiEvent)
                {
                    // Filter by user ID if specified
                    if (userIdSet is not null && !userIdSet.Contains(wikiEvent.UserId)) continue;

                    // Populate event properties
                    wikiEvent.Timestamp = eventMessage.ResolvedEvent.Event.Created;
                    wikiEvent.Id = eventMessage.ResolvedEvent.Event.EventNumber;
                    wikiEvent.UserId = wikiEvent.UserId;

                    // Apply pagination
                    if (skipped < skip)
                    {
                        skipped++;
                    }
                    else
                    {
                        events.Add(wikiEvent);
                        if (events.Count == take + 1) break;
                    }
                }
            }
        }

        // Determine if more events exist
        var hasMore = events.Count > take;
        if (hasMore)
            events = events.Take(take).ToList();

        return new PagedResult<BaseWikiEvent>
        {
            Data = events,
            Message = "Successfully retrieved events.",
            Success = true,
            Errors = [],
            Page = page,
            PageSize = pageSize,
            TotalCount = pageSize * (page - 1) + events.Count + (hasMore ? 1 : 0),
            TotalPages = hasMore ? page + 1 : page
        };
    }

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

    public async Task<bool> DeleteEvents(Ulid wikiId)
    {
        var result = await kurrentDbClient.DeleteAsync($"wiki-{wikiId}", StreamState.Any);
        return result.LogPosition == Position.Start;
    }
}
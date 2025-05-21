namespace Projeli.WikiService.Domain.Models.Events;

public class WikiCreatedEvent : BaseWikiEvent
{
    public WikiStatus Status { get; set; }
}
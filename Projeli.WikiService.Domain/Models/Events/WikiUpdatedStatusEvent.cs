namespace Projeli.WikiService.Domain.Models.Events;

public class WikiUpdatedStatusEvent : BaseWikiEvent
{
    public WikiStatus Status { get; set; }
}
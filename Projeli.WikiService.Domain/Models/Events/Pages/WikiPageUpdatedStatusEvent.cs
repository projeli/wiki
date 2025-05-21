namespace Projeli.WikiService.Domain.Models.Events.Pages;

public class WikiPageUpdatedStatusEvent : BaseWikiPageEvent
{
    public PageStatus Status { get; set; }
}
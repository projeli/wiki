namespace Projeli.WikiService.Domain.Models.Events.Pages;

public class WikiPageCreatedEvent : BaseWikiPageEvent
{
    public string Title { get; set; }
    public string Slug { get; set; }
}
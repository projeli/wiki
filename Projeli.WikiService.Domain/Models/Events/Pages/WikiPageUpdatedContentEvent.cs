namespace Projeli.WikiService.Domain.Models.Events.Pages;

public class WikiPageUpdatedContentEvent : BaseWikiPageEvent
{
    public string? Content { get; set; }
}
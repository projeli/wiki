namespace Projeli.WikiService.Domain.Models.Events;

public class WikiUpdatedContentEvent : BaseWikiEvent
{
    public string? Content { get; set; }
}
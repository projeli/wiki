namespace Projeli.WikiService.Domain.Models.Events.Pages;

public abstract class BaseWikiPageEvent : BaseWikiEvent
{
    public Ulid WikiPageId { get; set; }
}
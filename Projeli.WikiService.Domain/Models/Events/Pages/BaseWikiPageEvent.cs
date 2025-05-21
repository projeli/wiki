namespace Projeli.WikiService.Domain.Models.Events.Pages;

public class BaseWikiPageEvent : BaseWikiEvent
{
    public Ulid WikiPageId { get; set; }
}
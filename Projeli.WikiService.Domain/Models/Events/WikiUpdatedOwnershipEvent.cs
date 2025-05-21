namespace Projeli.WikiService.Domain.Models.Events;

public class WikiUpdatedOwnershipEvent : BaseWikiEvent
{
    public string ToUserId { get; set; }
}
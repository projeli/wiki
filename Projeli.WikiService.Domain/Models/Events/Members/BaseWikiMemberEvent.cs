namespace Projeli.WikiService.Domain.Models.Events.Members;

public abstract class BaseWikiMemberEvent : BaseWikiEvent
{
    public string MemberId { get; set; }
}
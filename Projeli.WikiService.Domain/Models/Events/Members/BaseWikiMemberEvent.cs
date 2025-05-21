namespace Projeli.WikiService.Domain.Models.Events.Members;

public class BaseWikiMemberEvent : BaseWikiEvent
{
    public string MemberId { get; set; }
}
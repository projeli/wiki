namespace Projeli.WikiService.Domain.Models.Events.Members;

public class WikiMemberUpdatedPermissionsEvent : BaseWikiMemberEvent
{
    public WikiMemberPermissions Permissions { get; set; }
}
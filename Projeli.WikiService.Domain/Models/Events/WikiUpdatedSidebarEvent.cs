namespace Projeli.WikiService.Domain.Models.Events;

public class WikiUpdatedSidebarEvent : BaseWikiEvent
{
    public WikiConfig.WikiConfigSidebar? Sidebar { get; set; }
}
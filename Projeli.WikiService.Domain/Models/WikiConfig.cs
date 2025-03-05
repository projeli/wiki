namespace Projeli.WikiService.Domain.Models;

public class WikiConfig
{
    public WikiConfigSidebar Sidebar { get; set; }
    
    public class WikiConfigSidebar
    {
        public List<WikiConfigSidebarItem> Items { get; set; } = [];
        
        public class WikiConfigSidebarItem
        {
            public string Title { get; set; }
            public string? Href { get; set; }
            public List<WikiConfigSidebarItem>? Category { get; set; } 
        }
    }
}
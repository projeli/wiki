namespace Projeli.WikiService.Domain.Models;

public class WikiConfig
{
    public WikiConfigSidebar Sidebar { get; set; }
    
    public class WikiConfigSidebar
    {
        public List<WikiConfigSidebarItem> Items { get; set; } = [];
        
        public class WikiConfigSidebarItem
        {
            public string Index { get; set; }
            public string Title { get; set; }
            public string? Slug { get; set; }
            public List<WikiConfigSidebarItem>? Category { get; set; } 
        }
    }
}
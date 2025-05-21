namespace Projeli.WikiService.Domain.Models.Events.Pages;

public class WikiPageUpdatedCategoriesEvent : BaseWikiPageEvent
{
    public List<SimpleCategory> Categories { get; set; }
    
    public class SimpleCategory
    {
        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
    }
}
namespace Projeli.WikiService.Domain.Models.Events.Categories;

public class WikiCategoryUpdatedPagesEvent : BaseWikiCategoryEvent
{
    public List<SimplePage> Pages { get; set; }
    
    public class SimplePage
    {
        public Ulid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}
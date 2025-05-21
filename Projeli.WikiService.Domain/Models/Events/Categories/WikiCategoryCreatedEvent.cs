namespace Projeli.WikiService.Domain.Models.Events.Categories;

public class WikiCategoryCreatedEvent : BaseWikiCategoryEvent
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
}
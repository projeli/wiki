namespace Projeli.WikiService.Domain.Models.Events.Categories;

public class WikiCategoryUpdatedEvent : BaseWikiCategoryEvent
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
}
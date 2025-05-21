namespace Projeli.WikiService.Domain.Models.Events.Categories;

public class BaseWikiCategoryEvent : BaseWikiEvent
{
    public Ulid CategoryId { get; set; }
}
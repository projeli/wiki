namespace Projeli.WikiService.Domain.Models.Events.Categories;

public abstract class BaseWikiCategoryEvent : BaseWikiEvent
{
    public Ulid CategoryId { get; set; }
}
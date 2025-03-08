namespace Projeli.WikiService.Application.Models.Requests;

public class UpdatePageCategoriesRequest
{
    public List<Ulid> CategoryIds { get; set; } = [];
}
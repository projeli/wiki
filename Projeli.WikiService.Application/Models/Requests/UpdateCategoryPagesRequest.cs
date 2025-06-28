namespace Projeli.WikiService.Application.Models.Requests;

public class UpdateCategoryPagesRequest
{
    public List<Ulid> PageIds { get; set; } = [];
}
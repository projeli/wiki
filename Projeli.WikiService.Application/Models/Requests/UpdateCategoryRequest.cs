namespace Projeli.WikiService.Application.Models.Requests;

public class UpdateCategoryRequest
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}
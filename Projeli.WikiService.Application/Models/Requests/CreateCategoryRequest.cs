namespace Projeli.WikiService.Application.Models.Requests;

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}
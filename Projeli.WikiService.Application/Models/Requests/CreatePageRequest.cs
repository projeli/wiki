namespace Projeli.WikiService.Application.Models.Requests;

public class CreatePageRequest
{
    public string Title { get; set; }
    public string Slug { get; set; }
}
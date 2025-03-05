namespace Projeli.WikiService.Application.Models.Requests;

public class UpdatePageRequest
{
    public string Title { get; set; }
    public string Slug { get; set; }
}
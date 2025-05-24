namespace Projeli.WikiService.Application.Models.Requests;

public class CreateWikiRequest
{
    public Ulid ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string ProjectSlug { get; set; }
    public string? ProjectImageUrl { get; set; }
    public List<CreateWikiMemberRequest> Members { get; set; }
}
namespace Projeli.WikiService.Application.Models.Responses;

public class SimpleWikiMemberResponse
{
    public Ulid Id { get; set; }
    
    public string UserId { get; set; }
    
    public bool IsOwner { get; set; }
}
namespace Projeli.WikiService.Application.Models.Requests;

public class CreateWikiMemberRequest
{
    public string UserId { get; set; }
    public bool IsOwner { get; set; }
}
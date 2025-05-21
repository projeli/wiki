using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Responses;

public class WikiMemberResponse
{
    public Ulid Id { get; set; }
    
    public string UserId { get; set; }
    
    public bool IsOwner { get; set; }
    
    public WikiMemberPermissions Permissions { get; set; }
}
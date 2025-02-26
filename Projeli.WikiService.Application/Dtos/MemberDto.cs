using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Dtos;

public class MemberDto
{
    public Ulid Id { get; set; }
    
    public Ulid WikiId { get; set; }
    
    public string UserId { get; set; }
    
    public bool IsOwner { get; set; }
    
    public WikiMemberPermissions Permissions { get; set; }
    
    
    public WikiDto Wiki { get; set; }
    public List<PageDto> Pages { get; set; } = [];
    public List<PageVersionDto> PageVersions { get; set; } = [];
}
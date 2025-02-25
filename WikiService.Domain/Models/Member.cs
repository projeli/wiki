using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WikiService.Domain.Models;

[Index(nameof(WikiId), nameof(UserId), IsUnique = true)]
public class Member
{
    [Key]
    public Ulid Id { get; set; }
    
    [ForeignKey(nameof(Wiki))]
    public Ulid WikiId { get; set; }
    
    [StringLength(32)]
    public string UserId { get; set; }
    
    public WikiMemberPermissions Permissions { get; set; }
    
    
    public Wiki Wiki { get; set; }
    public List<Page> Pages { get; set; } = [];
    public List<PageVersion> PageVersions { get; set; } = [];
}
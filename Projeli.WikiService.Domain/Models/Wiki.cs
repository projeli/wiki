using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Projeli.WikiService.Domain.Models;

[Index(nameof(ProjectId), IsUnique = true)]
public class Wiki
{
    [Key]
    public Ulid Id { get; set; }
    
    [Required]
    public Ulid ProjectId { get; set; }
    
    [Required, StringLength(32)]
    public string ProjectName { get; set; }
    
    [Required, StringLength(32)]
    public string ProjectSlug { get; set; }
    
    public string? Content { get; set; }
    
    public WikiConfig Config { get; set; }
    
    public WikiStatus Status { get; set; }
 
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public List<WikiMember> Members { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Page> Pages { get; set; } = [];
}
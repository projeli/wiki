using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WikiService.Domain.Models;

[Index(nameof(ProjectId), IsUnique = true)]
public class Wiki
{
    [Key]
    public Ulid Id { get; set; }
    
    [Required]
    public Ulid ProjectId { get; set; }
    
    public bool IsPublished { get; set; }
 
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public List<Member> Members { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Page> Pages { get; set; } = [];
}
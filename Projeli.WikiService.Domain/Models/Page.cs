using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Projeli.WikiService.Domain.Models;

[Index(nameof(WikiId), nameof(Slug), IsUnique = true)]
public class Page
{
    [Key]
    public Ulid Id { get; set; }
    
    [ForeignKey(nameof(Wiki))]
    public Ulid WikiId { get; set; }
    
    [Required, MaxLength(64)]
    public string Title { get; set; }
    
    [Required, MaxLength(64)]
    public string Slug { get; set; }
    
    public string? Content { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    
    public Wiki Wiki { get; set; }
    public List<Category> Categories { get; set; } = [];
    public List<PageVersion> Versions { get; set; } = [];
    public List<Member> Editors { get; set; } = [];
}
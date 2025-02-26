using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeli.WikiService.Domain.Models;

public class PageVersion
{
    [Key]
    public Ulid Id { get; set; }
    
    [ForeignKey(nameof(Page))]
    public Ulid PageId { get; set; }
    
    public int Version { get; set; }
    
    [Required, MaxLength(128)]
    public string Summary { get; set; }
    
    public string Content { get; set; }
    
    public string Difference { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    
    public Page Page { get; set; }
    public List<Member> Editors { get; set; } = [];
}
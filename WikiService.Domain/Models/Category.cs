using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WikiService.Domain.Models;

[Index(nameof(WikiId), nameof(Slug), IsUnique = true)]
public class Category
{
    [Key]
    public Ulid Id { get; set; }
    
    [ForeignKey(nameof(Wiki))]
    public Ulid WikiId { get; set; }
    
    [Required, MaxLength(32)]
    public string Name { get; set; }
    
    [Required, MaxLength(32)]
    public string Slug { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    
    public Wiki Wiki { get; set; }
    public List<Page> Pages { get; set; } = [];
}
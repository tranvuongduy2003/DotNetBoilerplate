using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Common.Entities;

public abstract class EntityAuditBase : EntityBase, IAuditable
{
    [Required]
    public required Guid AuthorId { get; set; }
    
    [ForeignKey("AuthorId")]
    [DeleteBehavior(DeleteBehavior.ClientSetNull)]
    public virtual User Author { get; set; } = null!;
}
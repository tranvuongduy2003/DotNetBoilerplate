using Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole<Guid>, IDateTracking, ISoftDeletable
{
    public Role(string name) : base(name)
    {
    }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
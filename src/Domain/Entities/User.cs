using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common.Interfaces;
using Domain.Enums.User;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IDateTracking, ISoftDeletable
{
    [MaxLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? FullName { get; set; }

    public DateTime? Dob { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EGender? Gender { get; set; }

    [MaxLength(1000)]
    [Column(TypeName = "nvarchar(1000)")]
    public string? Bio { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EUserStatus Status { get; set; } = EUserStatus.ACTIVE;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
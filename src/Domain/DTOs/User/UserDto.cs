using System.Text.Json.Serialization;
using Domain.Enums.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.DTOs.User;

public class UserDto
{
    [SwaggerSchema("Id of the user")]
    public Guid Id { get; set; }

    [SwaggerSchema("User name of the user")]
    public string UserName { get; set; }

    [SwaggerSchema("Email of the user")]
    public string Email { get; set; }

    [SwaggerSchema("Phone number of the user")]
    public string PhoneNumber { get; set; }

    [SwaggerSchema("Date of birth of the user")]
    public DateTime? Dob { get; set; }

    [SwaggerSchema("Full name of the user")]
    public string FullName { get; set; }

    [SwaggerSchema("Gender of the user")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EGender? Gender { get; set; }

    [SwaggerSchema("Biography of the user")]
    public string? Bio { get; set; }

    [SwaggerSchema("Status of the user's account")]
    public EUserStatus Status { get; set; }

    [SwaggerSchema("Roles of the user")]
    public IEnumerable<string> Roles { get; set; } = null;

    [SwaggerSchema("The datetime that the user was created")]
    public DateTime CreatedAt { get; set; }

    [SwaggerSchema("The last datetime that the user was updated")]
    public DateTime? UpdatedAt { get; set; }
}
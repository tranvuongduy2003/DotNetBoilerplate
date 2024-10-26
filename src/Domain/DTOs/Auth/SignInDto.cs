using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.DTOs.Auth;

public class SignInDto
{
    [DefaultValue("admin@gmail.com")]
    [SwaggerSchema("Phone number or email registered for the account")]
    public string Identity { get; set; }

    [DefaultValue("Admin@123")]
    [SwaggerSchema("Password to login the account")]
    public string Password { get; set; }
}
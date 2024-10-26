using Swashbuckle.AspNetCore.Annotations;

namespace Domain.DTOs.Auth;

public class RefreshTokenDto
{
    [SwaggerSchema("Refresh token received after login")]
    public string RefreshToken { get; set; }
}
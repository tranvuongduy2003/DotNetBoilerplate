using Swashbuckle.AspNetCore.Annotations;

namespace Domain.DTOs.Auth;

public class SignInResponseDto
{
    [SwaggerSchema("Access token to login the account")]
    public string AccessToken { get; set; }

    [SwaggerSchema("Refresh token to get another new access token")]
    public string RefreshToken { get; set; }
}
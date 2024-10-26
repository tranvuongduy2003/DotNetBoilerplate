using Domain.DTOs.Auth;
using Domain.DTOs.User;

namespace Domain.Abstractions.UseCases;

public interface IAuthUseCase
{
    Task<SignInResponseDto> SignUpAsync(CreateUserDto dto);
    
    Task<SignInResponseDto> SignInAsync(SignInDto dto);

    Task<bool> SignOutAsync();

    Task<SignInResponseDto> RefreshTokenAsync(string refreshToken, string accessToken);

    Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);

    Task<bool> ResetPasswordAsync(ResetUserPasswordDto dto);

    Task<UserDto> GetUserProfileAsync(Guid userId);
}
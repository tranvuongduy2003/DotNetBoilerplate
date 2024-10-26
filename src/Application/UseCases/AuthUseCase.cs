using Application.Exceptions;
using AutoMapper;
using Domain.Abstractions.Services;
using Domain.Abstractions.UseCases;
using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Domain.Entities;
using Domain.Enums.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.UseCases;

public class AuthUseCase : IAuthUseCase
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthUseCase(UserManager<User> userManager, IEmailService emailService, SignInManager<User> signInManager,
        ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<SignInResponseDto> SignUpAsync(CreateUserDto dto)
    {
        var useByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (useByEmail != null)
            throw new BadRequestException("Email already exists");

        var useByPhoneNumber = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (useByPhoneNumber != null)
            throw new BadRequestException("Phone number already exists");

        var user = new User
        {
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            FullName = dto.FullName,
            UserName = dto.UserName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded)
        {
            var userToReturn = await _userManager.FindByEmailAsync(dto.Email);

            await _userManager.AddToRolesAsync(user, new List<string>
            {
                nameof(EUserRole.CUSTOMER)
            });

            await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken =
                await _userManager
                    .GenerateUserTokenAsync(user, TokenProviders.DEFAULT, TokenTypes.REFRESH);

            await _userManager
                .SetAuthenticationTokenAsync(user, TokenProviders.DEFAULT, TokenTypes.REFRESH, refreshToken);

            var signUpResponse = new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            await _emailService
                .SendRegistrationConfirmationEmailAsync(userToReturn.Email, userToReturn.FullName);

            return signUpResponse;
        }

        throw new BadRequestException(result);
    }

    public async Task<SignInResponseDto> SignInAsync(SignInDto dto)
    {
        var user = _userManager.Users.FirstOrDefault(u =>
            u.Email == dto.Identity || u.PhoneNumber == dto.Identity);
        if (user == null)
            throw new NotFoundException("Invalid credentials");

        var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (isValid == false) throw new UnauthorizedException("Invalid credentials");

        if (user.Status == EUserStatus.INACTIVE)
            throw new UnauthorizedException("Your account was disabled");

        await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

        var accessToken = await _tokenService
            .GenerateAccessTokenAsync(user);
        var refreshToken = await _userManager
            .GenerateUserTokenAsync(user, TokenProviders.DEFAULT, TokenTypes.REFRESH);

        await _userManager.SetAuthenticationTokenAsync(user, TokenProviders.DEFAULT, TokenTypes.REFRESH, refreshToken);

        var signInResponse = new SignInResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return signInResponse;
    }

    public async Task<bool> SignOutAsync()
    {
        await _signInManager.SignOutAsync();

        return true;
    }

    public async Task<SignInResponseDto> RefreshTokenAsync(string refreshToken, string accessToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new InvalidTokenException();

        if (string.IsNullOrEmpty(accessToken)) throw new UnauthorizedException("Unauthorized");
        var principal = _tokenService.GetPrincipalFromToken(accessToken);

        var user = await _userManager.FindByIdAsync(principal.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value);
        if (user == null) throw new UnauthorizedException("Unauthorized");

        var isValid = await _userManager.VerifyUserTokenAsync(
            user,
            TokenProviders.DEFAULT,
            TokenTypes.REFRESH,
            refreshToken);
        if (!isValid) throw new UnauthorizedException("Unauthorized");

        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var newRefreshToken =
            await _userManager.GenerateUserTokenAsync(user, TokenProviders.DEFAULT, TokenTypes.REFRESH);

        var refreshResponse = new SignInResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return refreshResponse;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User does not exist");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordUrl = $"https://localhost:5173/reset-password?token={token}&email={dto.Email}";

        await _emailService
            .SendResetPasswordEmailAsync(dto.Email, resetPasswordUrl);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetUserPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User does not exist");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded) throw new BadRequestException(result);

        return true;
    }

    public async Task<UserDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new UnauthorizedException("Unauthorized");

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles.ToList();

        return userDto;
    }
}
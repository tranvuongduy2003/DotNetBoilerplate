using Application.Exceptions;
using Domain.Abstractions.UseCases;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Domain.HttpResponses;
using Infrastructure.FilterAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthUseCase _authUseCase;

    public AuthController(ILogger<AuthController> logger, IAuthUseCase authUseCase)
    {
        _logger = logger;
        _authUseCase = authUseCase;
    }

    [HttpPost("signup")]
    [SwaggerOperation(
        Summary = "Sign up a new user",
        Description =
            "Registers a new user with the provided details. Returns a sign-in response upon successful registration."
    )]
    [SwaggerResponse(200, "User successfully registered", typeof(SignInResponseDto))]
    [SwaggerResponse(400, "Invalid user input")]
    [SwaggerResponse(500, "An error occurred while processing the request")]
    [ApiValidationFilter]
    public async Task<IActionResult> SignUp([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("START: SignUp");
        try
        {
            var signUpResponse = await _authUseCase.SignUpAsync(dto);

            _logger.LogInformation("END: SignUp");

            return Ok(new ApiOkResponse(signUpResponse));
        }
        catch (BadRequestException e)
        {
            return BadRequest(new ApiBadRequestResponse(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost("signin")]
    [SwaggerOperation(
        Summary = "Sign in a user",
        Description =
            "Authenticates the user based on the provided credentials and returns a sign-in response if successful."
    )]
    [SwaggerResponse(200, "Successfully signed in", typeof(SignInResponseDto))]
    [SwaggerResponse(401, "Unauthorized - Invalid credentials")]
    [SwaggerResponse(404, "Not Found - User not found")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        _logger.LogInformation("START: SignIn");
        try
        {
            var signInResponse = await _authUseCase.SignInAsync(dto);

            _logger.LogInformation("END: SignIn");

            return Ok(new ApiOkResponse(signInResponse));
        }
        catch (NotFoundException e)
        {
            return NotFound(new ApiNotFoundResponse(e.Message));
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(new ApiUnauthorizedResponse(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost("signout")]
    [SwaggerOperation(
        Summary = "Sign out a user",
        Description = "Signs out the current user, invalidating their session or authentication token."
    )]
    [SwaggerResponse(200, "Successfully signed out")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    public async Task<IActionResult> SignOut()
    {
        _logger.LogInformation("START: SignOut");
        try
        {
            await _authUseCase.SignOutAsync();

            _logger.LogInformation("END: SignOut");

            Response.Cookies.Delete("AuthTokenHolder");

            return Ok(new ApiOkResponse());
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh user authentication token",
        Description = "Refreshes the user's authentication token based on the provided refresh token credentials."
    )]
    [SwaggerResponse(200, "Successfully refreshed the token", typeof(SignInResponseDto))]
    [SwaggerResponse(401, "Unauthorized - Invalid or expired refresh token")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    [TokenRequirementFilter]
    [ApiValidationFilter]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        _logger.LogInformation("START: RefreshToken");
        try
        {
            var accessToken = Request
                .Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");

            var refreshTokenResponse = await _authUseCase.RefreshTokenAsync(dto.RefreshToken, accessToken);

            _logger.LogInformation("END: RefreshToken");

            return Ok(new ApiOkResponse(refreshTokenResponse));
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(new ApiUnauthorizedResponse(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost("forgot-password")]
    [SwaggerOperation(
        Summary = "Initiate password recovery",
        Description = "Sends a password recovery email to the user based on the provided email address."
    )]
    [SwaggerResponse(200, "Password recovery email sent successfully")]
    [SwaggerResponse(401, "Unauthorized - Invalid or missing credentials")]
    [SwaggerResponse(404, "Not Found - User with the provided email address not found")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    [TokenRequirementFilter]
    [ApiValidationFilter]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        _logger.LogInformation("START: ForgotPassword");
        try
        {
            await _authUseCase.ForgotPasswordAsync(dto);

            _logger.LogInformation("END: ForgotPassword");

            return Ok(new ApiOkResponse());
        }
        catch (NotFoundException e)
        {
            return NotFound(new ApiNotFoundResponse(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(
        Summary = "Reset user password",
        Description = "Resets the user's password based on the provided credentials and reset information."
    )]
    [SwaggerResponse(200, "Password reset successfully")]
    [SwaggerResponse(400, "Bad Request - Invalid input or parameters")]
    [SwaggerResponse(401, "Unauthorized - Invalid or expired credentials")]
    [SwaggerResponse(404, "Not Found - User or resource not found")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    [TokenRequirementFilter]
    [ApiValidationFilter]
    public async Task<IActionResult> ResetPassword([FromBody] ResetUserPasswordDto dto)
    {
        _logger.LogInformation("START: ResetPassword");
        try
        {
            await _authUseCase.ResetPasswordAsync(dto);

            _logger.LogInformation("END: ResetPassword");

            return Ok(new ApiOkResponse());
        }
        catch (NotFoundException e)
        {
            return NotFound(new ApiNotFoundResponse(e.Message));
        }
        catch (BadRequestException e)
        {
            return BadRequest(new BadRequestException(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet("profile")]
    [SwaggerOperation(
        Summary = "Retrieve user profile",
        Description = "Fetches the details of the currently authenticated user."
    )]
    [SwaggerResponse(200, "User profile retrieved successfully", typeof(UserDto))]
    [SwaggerResponse(401, "Unauthorized - User not authenticated")]
    [SwaggerResponse(403, "Forbidden - User does not have the required permissions")]
    [SwaggerResponse(500, "Internal Server Error - An error occurred while processing the request")]
    [TokenRequirementFilter]
    public async Task<IActionResult> GetUserProfile()
    {
        _logger.LogInformation("START: GetUserProfile");
        try
        {
            var userId = (Guid)HttpContext.Items["AuthorId"];

            var user = await _authUseCase.GetUserProfileAsync(userId);

            _logger.LogInformation("END: GetUserProfile");

            return Ok(new ApiOkResponse(user));
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(new ApiUnauthorizedResponse(e.Message));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
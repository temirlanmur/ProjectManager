using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.WebAPI.Authentication.Service;
using ProjectManager.WebAPI.ViewModels;

namespace ProjectManager.WebAPI.Controllers;

[Route("auth")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Registers new user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">User information with Bearer token.</response>
    [AllowAnonymous]
    [HttpPost("signup")]
    [ProducesResponseType(typeof(AuthenticationViewModel), 200)]
    public async Task<IActionResult> Register(RegisterViewModel request)
    {
        AuthenticationResult result = await _authenticationService.RegisterAsync(new RegisterDTO(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName));

        return Ok(new AuthenticationViewModel(
                result.UserId,
                result.FirstName,
                result.LastName,
                result.Email,
                result.Token));
    }

    /// <summary>
    /// Logins user to the system.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">User information with Bearer token.</response>
    [AllowAnonymous]
    [HttpPost("signin")]
    [ProducesResponseType(typeof(AuthenticationViewModel), 200)]
    public async Task<IActionResult> Login(LoginViewModel request)
    {
        AuthenticationResult result = await _authenticationService.LoginAsync(new LoginDTO(
            request.Email,
            request.Password));

        return Ok(new AuthenticationViewModel(
                result.UserId,
                result.FirstName,
                result.LastName,
                result.Email,
                result.Token));
    }

    /// <summary>
    /// Helper method to check token validity.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Indicates that the token is valid.</response>
    /// <response code="401">User is unauthenticated.</response>
    [HttpGet("check")]
    public IActionResult CheckToken()
    {
        return Ok();
    }
}

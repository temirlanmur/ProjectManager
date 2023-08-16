namespace ProjectManager.WebAPI.Authentication.Service;

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterAsync(RegisterDTO dto);
    Task<AuthenticationResult> LoginAsync(LoginDTO dto);
}

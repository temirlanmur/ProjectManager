using FluentValidation;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.WebAPI.Authentication.Utils;
using ProjectManager.WebAPI.Exceptions;

namespace ProjectManager.WebAPI.Authentication.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginDTO> _loginDtoValidator;
    private readonly IValidator<RegisterDTO> _registerDtoValidator;

    public AuthenticationService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<LoginDTO> loginDtoValidator,
        IValidator<RegisterDTO> registerDtoValidator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _loginDtoValidator = loginDtoValidator;
        _registerDtoValidator = registerDtoValidator;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginDTO dto)
    {
        await _loginDtoValidator.ValidateAndThrowAsync(dto);

        if (await _userRepository.GetByEmailAsync(dto.Email) is not User user)
        {
            throw new InvalidCredentialsException();
        }

        if (user.Password != dto.Password)
        {
            throw new InvalidCredentialsException();
        }

        string token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            token);
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterDTO dto)
    {
        await _registerDtoValidator.ValidateAndThrowAsync(dto);

        if (await _userRepository.GetByEmailAsync(dto.Email) is not null)
        {
            throw new DuplicateEmailException();
        }

        User user = new(dto.FirstName, dto.LastName, dto.Email, dto.Password);

        await _userRepository.SaveAsync(user);

        string token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            token);
    }
}

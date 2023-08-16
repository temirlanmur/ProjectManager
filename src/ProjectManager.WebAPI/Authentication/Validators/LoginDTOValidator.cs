using FluentValidation;
using ProjectManager.WebAPI.Authentication.Service;

namespace ProjectManager.WebAPI.Authentication.Validators;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(d => d.Email).EmailAddress();
        RuleFor(d => d.Password).Length(8, 32);
    }
}

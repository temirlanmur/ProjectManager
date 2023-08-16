using FluentValidation;
using ProjectManager.WebAPI.Authentication.Service;

namespace ProjectManager.WebAPI.Authentication.Validators;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
    {
        RuleFor(d => d.Email).EmailAddress();
        RuleFor(d => d.Password).Length(8, 32);
        RuleFor(d => d.FirstName).MinimumLength(1);
        RuleFor(d => d.LastName).MinimumLength(1);
    }
}

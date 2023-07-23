using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;

namespace ProjectManager.Application.Validators;

public class CreateProjectDTOValidator : AbstractValidator<CreateProjectDTO>
{
    public CreateProjectDTOValidator()
    {
        RuleFor(d => d.ownerId).NotEmpty();
        RuleFor(d => d.title).Length(3, 255);
        RuleFor(d => d.description).MaximumLength(2000);
    }
}

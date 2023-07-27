using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;

namespace ProjectManager.Application.Validators;

public class UpdateProjectDTOValidator : AbstractValidator<UpdateProjectDTO>
{
    public UpdateProjectDTOValidator()
    {
        RuleFor(d => d.ActorId).NotEmpty();
        RuleFor(d => d.ProjectId).NotEmpty();
        RuleFor(d => d.Title).Length(3, 255);
        RuleFor(d => d.Description).MaximumLength(2000);
    }
}

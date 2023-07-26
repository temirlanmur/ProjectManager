using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;

namespace ProjectManager.Application.Validators;

public class CreateTaskDTOValidator : AbstractValidator<CreateTaskDTO>
{
    public CreateTaskDTOValidator()
    {
        RuleFor(d => d.ActorId).NotEmpty();
        RuleFor(d => d.ProjectId).NotEmpty();
        RuleFor(d => d.Title).Length(3, 255);
        RuleFor(d => d.Description).MaximumLength(2000);
    }
}

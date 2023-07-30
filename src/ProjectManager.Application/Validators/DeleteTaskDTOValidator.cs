using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;

namespace ProjectManager.Application.Validators;

public class DeleteTaskDTOValidator : AbstractValidator<DeleteTaskDTO>
{
    public DeleteTaskDTOValidator()
    {
        RuleFor(d => d.ActorId).NotEmpty();
        RuleFor(d => d.ProjectId).NotEmpty();
        RuleFor(d => d.TaskId).NotEmpty();
    }
}

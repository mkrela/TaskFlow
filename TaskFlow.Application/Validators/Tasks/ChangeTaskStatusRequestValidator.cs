using FluentValidation;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Validators.Tasks;

public sealed class ChangeTaskStatusRequestValidator : AbstractValidator<ChangeTaskStatusRequest>
{
    public ChangeTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(status => Enum.IsDefined(typeof(TaskFlow.Domain.Enums.TaskStatus), status))
            .WithMessage("Le statut fourni est invalide.");
    }
}
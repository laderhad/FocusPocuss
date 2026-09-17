namespace FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;

public class CreateTaskStartPlanCommandValidator : AbstractValidator<CreateTaskStartPlanCommand>
{
    public CreateTaskStartPlanCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .GreaterThan(0);

        RuleFor(command => command.Language)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(language => language is "tr" or "en")
            .WithMessage("Language must be either 'tr' or 'en'.");
    }
}

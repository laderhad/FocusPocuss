namespace FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;

public class StartFocusSessionCommandValidator : AbstractValidator<StartFocusSessionCommand>
{
    public StartFocusSessionCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .GreaterThan(0);

        RuleFor(command => command.TaskStartPlanId)
            .GreaterThan(0);
    }
}

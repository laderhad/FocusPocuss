namespace FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;

public class CompleteFocusSessionCommandValidator : AbstractValidator<CompleteFocusSessionCommand>
{
    public CompleteFocusSessionCommandValidator()
    {
        RuleFor(command => command.FocusSessionId)
            .GreaterThan(0);
    }
}

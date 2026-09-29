namespace FocusPocuss.Application.FocusSessions.Commands.RecordFocusSessionReflection;

public class RecordFocusSessionReflectionCommandValidator
    : AbstractValidator<RecordFocusSessionReflectionCommand>
{
    public RecordFocusSessionReflectionCommandValidator()
    {
        RuleFor(command => command.FocusSessionId)
            .GreaterThan(0);

        RuleFor(command => command.Reflection)
            .IsInEnum();
    }
}

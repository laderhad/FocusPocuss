namespace FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;

public class ReportDistractionCommandValidator : AbstractValidator<ReportDistractionCommand>
{
    public ReportDistractionCommandValidator()
    {
        RuleFor(command => command.FocusSessionId)
            .GreaterThan(0);

        RuleFor(command => command.Language).Must(language => language is "tr" or "en");

        RuleFor(command => command.Reason)
            .IsInEnum();
    }
}

namespace FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;

public class GetFocusSessionQueryValidator : AbstractValidator<GetFocusSessionQuery>
{
    public GetFocusSessionQueryValidator()
    {
        RuleFor(query => query.FocusSessionId)
            .GreaterThan(0);
    }
}

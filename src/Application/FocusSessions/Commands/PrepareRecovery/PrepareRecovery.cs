using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.PrepareRecovery;

[Authorize]
public record PrepareRecoveryCommand(int FocusSessionId, int DistractionId,
    RecoveryChoice? Choice = null, string? Clarification = null) : IRequest<DistractionReportDto>;

public sealed class PrepareRecoveryCommandValidator : AbstractValidator<PrepareRecoveryCommand>
{
    public PrepareRecoveryCommandValidator()
    {
        RuleFor(x => x.FocusSessionId).GreaterThan(0);
        RuleFor(x => x.DistractionId).GreaterThan(0);
        RuleFor(x => x.Choice).IsInEnum();
        RuleFor(x => x.Clarification).MaximumLength(500);
        RuleFor(x => x).Must(x => (x.Choice is not null) != !string.IsNullOrWhiteSpace(x.Clarification));
        RuleFor(x => x.Choice).NotEqual(RecoveryChoice.EndSession);
    }
}

public sealed class PrepareRecoveryCommandHandler(IApplicationDbContext context, IUser user,
    RecoveryActionPreparation preparation) : IRequestHandler<PrepareRecoveryCommand, DistractionReportDto>
{
    public async Task<DistractionReportDto> Handle(PrepareRecoveryCommand request, CancellationToken cancellationToken)
    {
        var session = await context.FocusSessions.Include(x => x.DistractionEvents)
            .SingleOrDefaultAsync(x => x.Id == request.FocusSessionId && x.UserId == user.Id
                && x.CompletedAtUtc == null && x.EndedEarlyAtUtc == null, cancellationToken);
        Guard.Against.NotFound(request.FocusSessionId, session);
        var recovery = session.DistractionEvents.SingleOrDefault(x => x.Id == request.DistractionId && x.InterventionType != null);
        Guard.Against.NotFound(request.DistractionId, recovery);
        if (recovery.ResolvedAtUtc is not null || recovery.ActionAtDistraction != session.CurrentAction)
            throw new ConflictException();
        session.TouchRecovery();
        if (request.Choice is { } choice)
        {
            if (recovery.Requirement != RecoveryRequirement.UserChoice) throw new ConflictException();
            recovery.SelectChoice(choice);
            if (choice == RecoveryChoice.ContinueWithSmallerAction)
                await preparation.PrepareAsync(recovery, null, cancellationToken);
        }
        else
        {
            if (recovery.Requirement != RecoveryRequirement.NeedsClarification || recovery.ClarificationUsed)
                throw new ConflictException();
            recovery.UseClarification();
            await preparation.PrepareAsync(recovery, request.Clarification!.Trim(), cancellationToken);
        }
        await context.SaveChangesAsync(cancellationToken);
        return DistractionReportDto.FromEntity(recovery);
    }
}

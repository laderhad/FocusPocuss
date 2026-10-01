using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.ResolveRecovery;

[Authorize]
public record ResolveRecoveryCommand(int FocusSessionId, int DistractionId,
    RecoveryResolution Resolution, string? Thought = null) : IRequest<FocusSessionDto>;

public sealed class ResolveRecoveryCommandValidator : AbstractValidator<ResolveRecoveryCommand>
{
    public ResolveRecoveryCommandValidator()
    {
        RuleFor(x => x.FocusSessionId).GreaterThan(0);
        RuleFor(x => x.DistractionId).GreaterThan(0);
        RuleFor(x => x.Resolution).IsInEnum();
        RuleFor(x => x.Thought).MaximumLength(1000);
    }
}

public sealed class ResolveRecoveryCommandHandler(IApplicationDbContext context, IUser user,
    TimeProvider clock) : IRequestHandler<ResolveRecoveryCommand, FocusSessionDto>
{
    public async Task<FocusSessionDto> Handle(ResolveRecoveryCommand request, CancellationToken cancellationToken)
    {
        var session = await context.FocusSessions.Include(x => x.DistractionEvents)
            .SingleOrDefaultAsync(x => x.Id == request.FocusSessionId && x.UserId == user.Id, cancellationToken);
        Guard.Against.NotFound(request.FocusSessionId, session);
        var recovery = session.DistractionEvents.SingleOrDefault(x => x.Id == request.DistractionId && x.InterventionType != null);
        Guard.Against.NotFound(request.DistractionId, recovery);
        if (recovery.ResolvedAtUtc is not null)
        {
            if (recovery.Resolution != request.Resolution) throw new ConflictException();
            return FocusSessionDto.FromEntity(session); // Retried response, never reapply an old action.
        }
        if (session.CompletedAtUtc is not null || session.EndedEarlyAtUtc is not null
            || recovery.ActionAtDistraction != session.CurrentAction) throw new ConflictException();
        if (request.Resolution == RecoveryResolution.EndSession
            && recovery.InterventionType != RecoveryInterventionType.RecoveryChoice) throw new ConflictException();
        if (request.Resolution == RecoveryResolution.ReturnToFocus)
        {
            if (recovery.Requirement == RecoveryRequirement.ThoughtCapture)
            {
                if (string.IsNullOrWhiteSpace(request.Thought))
                    throw new Common.Exceptions.ValidationException([new("Thought", "A thought is required.")]);
            }
            else if (recovery.Requirement != RecoveryRequirement.None || recovery.ProposedAction is null)
                throw new ConflictException();
        }
        recovery.Resolve(request.Resolution, clock.GetUtcNow(), request.Thought);
        if (request.Resolution == RecoveryResolution.EndSession) session.EndEarly(clock.GetUtcNow());
        else if (request.Resolution == RecoveryResolution.ReturnToFocus)
            session.ApplyRecoveryAction(recovery.ProposedAction!);
        else session.TouchRecovery();
        await context.SaveChangesAsync(cancellationToken);
        return FocusSessionDto.FromEntity(session);
    }
}

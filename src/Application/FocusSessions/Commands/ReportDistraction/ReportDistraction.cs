using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;

[Authorize]
public record ReportDistractionCommand(
    int FocusSessionId,
    DistractionReason Reason, string Language = "en") : IRequest<DistractionReportDto>;

public class ReportDistractionCommandHandler
    : IRequestHandler<ReportDistractionCommand, DistractionReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly DistractionInterventionSelector _interventionSelector;
    private readonly RecoveryActionPreparation _preparation;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public ReportDistractionCommandHandler(
        IApplicationDbContext context,
        DistractionInterventionSelector interventionSelector,
        TimeProvider timeProvider,
        RecoveryActionPreparation preparation,
        IUser user)
    {
        _preparation = preparation;
        _context = context;
        _interventionSelector = interventionSelector;
        _timeProvider = timeProvider;
        _user = user;
    }

    public async Task<DistractionReportDto> Handle(
        ReportDistractionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context.FocusSessions
            .Include(item => item.DistractionEvents)
            .SingleOrDefaultAsync(
                item => item.Id == request.FocusSessionId
                    && item.UserId == _user.Id
                    && item.CompletedAtUtc == null && item.EndedEarlyAtUtc == null,
                cancellationToken);

        Guard.Against.NotFound(request.FocusSessionId, session);

        var pending = session.DistractionEvents.SingleOrDefault(item => item.InterventionType != null && item.ResolvedAtUtc == null);
        if (pending is not null) return DistractionReportDto.FromEntity(pending);

        session.TouchRecovery();
        var intervention = _interventionSelector.Select(session, request.Reason);

        var distractionEvent = new DistractionEvent(
            session.Id,
            request.Reason,
            _timeProvider.GetUtcNow());

        distractionEvent.BeginRecovery(intervention.Type, intervention.Version, session.CurrentAction,
            request.Language, intervention.Requirement);
        if (intervention.Requirement == RecoveryRequirement.ActionTransformation)
            await _preparation.PrepareAsync(distractionEvent, null, cancellationToken);

        _context.DistractionEvents.Add(distractionEvent);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Another tab may have reported first. Resume its persisted recovery,
            // but never hide a failure if no active, owned recovery actually exists.
            var concurrent = await _context.DistractionEvents.AsNoTracking()
                .SingleOrDefaultAsync(item => item.FocusSessionId == session.Id
                    && item.InterventionType != null && item.ResolvedAtUtc == null
                    && item.FocusSession.UserId == _user.Id
                    && item.FocusSession.CompletedAtUtc == null && item.FocusSession.EndedEarlyAtUtc == null,
                    cancellationToken);
            if (concurrent is not null) return DistractionReportDto.FromEntity(concurrent);
            throw;
        }

        return DistractionReportDto.FromEntity(distractionEvent);
    }
}

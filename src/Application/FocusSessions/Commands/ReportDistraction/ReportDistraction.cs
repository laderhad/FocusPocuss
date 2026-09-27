using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;

[Authorize]
public record ReportDistractionCommand(
    int FocusSessionId,
    DistractionReason Reason) : IRequest<DistractionReportDto>;

public class ReportDistractionCommandHandler
    : IRequestHandler<ReportDistractionCommand, DistractionReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly DistractionInterventionSelector _interventionSelector;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public ReportDistractionCommandHandler(
        IApplicationDbContext context,
        DistractionInterventionSelector interventionSelector,
        TimeProvider timeProvider,
        IUser user)
    {
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
            .SingleOrDefaultAsync(
                item => item.Id == request.FocusSessionId
                    && item.UserId == _user.Id
                    && item.CompletedAtUtc == null,
                cancellationToken);

        Guard.Against.NotFound(request.FocusSessionId, session);

        var strategy = _interventionSelector.Select(request.Reason);

        var distractionEvent = new DistractionEvent(
            session.Id,
            request.Reason,
            _timeProvider.GetUtcNow());

        _context.DistractionEvents.Add(distractionEvent);

        await _context.SaveChangesAsync(cancellationToken);

        return DistractionReportDto.FromEntity(distractionEvent, strategy);
    }
}

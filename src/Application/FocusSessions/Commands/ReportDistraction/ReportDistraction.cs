using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;

[Authorize]
public record ReportDistractionCommand(
    int FocusSessionId,
    DistractionReason Reason) : IRequest<DistractionEventDto>;

public class ReportDistractionCommandHandler
    : IRequestHandler<ReportDistractionCommand, DistractionEventDto>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public ReportDistractionCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        IUser user)
    {
        _context = context;
        _timeProvider = timeProvider;
        _user = user;
    }

    public async Task<DistractionEventDto> Handle(
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

        var distractionEvent = new DistractionEvent(
            session.Id,
            request.Reason,
            _timeProvider.GetUtcNow());

        _context.DistractionEvents.Add(distractionEvent);

        await _context.SaveChangesAsync(cancellationToken);

        return DistractionEventDto.FromEntity(distractionEvent);
    }
}

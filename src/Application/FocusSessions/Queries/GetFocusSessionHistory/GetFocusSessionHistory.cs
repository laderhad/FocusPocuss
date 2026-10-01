using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;

namespace FocusPocuss.Application.FocusSessions.Queries.GetFocusSessionHistory;

[Authorize]
public record GetFocusSessionHistoryQuery
    : IRequest<IReadOnlyList<FocusSessionHistoryItemDto>>;

public class GetFocusSessionHistoryQueryHandler
    : IRequestHandler<GetFocusSessionHistoryQuery, IReadOnlyList<FocusSessionHistoryItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetFocusSessionHistoryQueryHandler(
        IApplicationDbContext context,
        IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<IReadOnlyList<FocusSessionHistoryItemDto>> Handle(
        GetFocusSessionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _user.Id!;

        return await _context.FocusSessions
            .AsNoTracking()
            .Where(session => session.UserId == userId)
            .OrderByDescending(session => session.StartedAtUtc)
            .ThenByDescending(session => session.Id)
            .Select(session => new FocusSessionHistoryItemDto
            {
                Id = session.Id,
                ParkedThoughtCount = session.DistractionEvents.Count(item => item.ParkedThought != null),
                TaskId = session.TaskItemId,
                Action = session.RecoveryAction ?? session.Action,
                PlannedDurationMinutes = session.PlannedDurationMinutes,
                StartedAtUtc = session.StartedAtUtc,
                CompletedAtUtc = session.CompletedAtUtc,
                EndedEarlyAtUtc = session.EndedEarlyAtUtc,
                Reflection = session.Reflection,
                ReflectedAtUtc = session.ReflectedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}

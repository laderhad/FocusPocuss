using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;

[Authorize]
public record StartFocusSessionCommand(int TaskId, int TaskStartPlanId) : IRequest<FocusSessionDto>;

public class StartFocusSessionCommandHandler : IRequestHandler<StartFocusSessionCommand, FocusSessionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public StartFocusSessionCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        IUser user)
    {
        _context = context;
        _timeProvider = timeProvider;
        _user = user;
    }

    public async Task<FocusSessionDto> Handle(
        StartFocusSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _user.Id!;
        var startPlan = await _context.TaskStartPlans
            .AsNoTracking()
            .Include(plan => plan.TaskItem)
            .SingleOrDefaultAsync(
                plan => plan.Id == request.TaskStartPlanId
                    && plan.TaskItemId == request.TaskId
                    && plan.TaskItem.UserId == userId,
                cancellationToken);

        Guard.Against.NotFound(request.TaskStartPlanId, startPlan);

        var activeSession = await GetActiveSessionAsync(userId, cancellationToken);
        if (activeSession is not null)
        {
            return FocusSessionDto.FromEntity(activeSession);
        }

        var session = new FocusSession(
            userId,
            startPlan.TaskItemId,
            startPlan.Id,
            startPlan.NextAction,
            startPlan.SuggestedDurationMinutes,
            _timeProvider.GetUtcNow());

        _context.FocusSessions.Add(session);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return FocusSessionDto.FromEntity(session);
        }
        catch (DbUpdateException)
        {
            var concurrentSession = await GetActiveSessionAsync(userId, cancellationToken);
            if (concurrentSession is not null)
            {
                return FocusSessionDto.FromEntity(concurrentSession);
            }

            throw;
        }
    }

    private Task<FocusSession?> GetActiveSessionAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return _context.FocusSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                session => session.UserId == userId && session.CompletedAtUtc == null,
                cancellationToken);
    }
}

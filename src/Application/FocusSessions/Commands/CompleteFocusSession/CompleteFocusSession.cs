using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;

namespace FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;

[Authorize]
public record CompleteFocusSessionCommand(int FocusSessionId) : IRequest<FocusSessionDto>;

public class CompleteFocusSessionCommandHandler : IRequestHandler<CompleteFocusSessionCommand, FocusSessionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public CompleteFocusSessionCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        IUser user)
    {
        _context = context;
        _timeProvider = timeProvider;
        _user = user;
    }

    public async Task<FocusSessionDto> Handle(
        CompleteFocusSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context.FocusSessions
            .SingleOrDefaultAsync(
                item => item.Id == request.FocusSessionId && item.UserId == _user.Id,
                cancellationToken);

        Guard.Against.NotFound(request.FocusSessionId, session);

        if (session.CompletedAtUtc is null)
        {
            session.Complete(_timeProvider.GetUtcNow());
            await _context.SaveChangesAsync(cancellationToken);
        }

        return FocusSessionDto.FromEntity(session);
    }
}

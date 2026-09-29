using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions.Commands.RecordFocusSessionReflection;

[Authorize]
public record RecordFocusSessionReflectionCommand(
    int FocusSessionId,
    FocusSessionReflection Reflection) : IRequest<FocusSessionDto>;

public class RecordFocusSessionReflectionCommandHandler
    : IRequestHandler<RecordFocusSessionReflectionCommand, FocusSessionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly IUser _user;

    public RecordFocusSessionReflectionCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        IUser user)
    {
        _context = context;
        _timeProvider = timeProvider;
        _user = user;
    }

    public async Task<FocusSessionDto> Handle(
        RecordFocusSessionReflectionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context.FocusSessions
            .SingleOrDefaultAsync(
                item => item.Id == request.FocusSessionId
                    && item.UserId == _user.Id
                    && item.CompletedAtUtc != null,
                cancellationToken);

        Guard.Against.NotFound(request.FocusSessionId, session);

        session.RecordReflection(request.Reflection, _timeProvider.GetUtcNow());

        await _context.SaveChangesAsync(cancellationToken);

        return FocusSessionDto.FromEntity(session);
    }
}

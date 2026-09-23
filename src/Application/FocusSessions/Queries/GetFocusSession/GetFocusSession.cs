using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;

namespace FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;

[Authorize]
public record GetFocusSessionQuery(int FocusSessionId) : IRequest<FocusSessionDto>;

public class GetFocusSessionQueryHandler : IRequestHandler<GetFocusSessionQuery, FocusSessionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetFocusSessionQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<FocusSessionDto> Handle(
        GetFocusSessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _context.FocusSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == request.FocusSessionId && item.UserId == _user.Id,
                cancellationToken);

        Guard.Against.NotFound(request.FocusSessionId, session);

        return FocusSessionDto.FromEntity(session);
    }
}

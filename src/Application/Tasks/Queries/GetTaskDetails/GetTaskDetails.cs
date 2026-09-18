using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;

namespace FocusPocuss.Application.Tasks.Queries.GetTaskDetails;

[Authorize]
public record GetTaskDetailsQuery(int TaskId) : IRequest<TaskDetailsDto>;

public class GetTaskDetailsQueryHandler : IRequestHandler<GetTaskDetailsQuery, TaskDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetTaskDetailsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<TaskDetailsDto> Handle(
        GetTaskDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .AsNoTracking()
            .Where(item => item.Id == request.TaskId && item.UserId == _user.Id)
            .Select(item => new TaskDetailsDto
            {
                Id = item.Id,
                OriginalInput = item.OriginalInput,
                CreatedAt = item.Created,
                StartPlan = item.StartPlan == null
                    ? null
                    : new TaskStartPlanDto
                    {
                        Id = item.StartPlan.Id,
                        Message = item.StartPlan.Message,
                        NextAction = item.StartPlan.NextAction,
                        SuggestedDurationMinutes = item.StartPlan.SuggestedDurationMinutes,
                        CreatedAt = item.StartPlan.Created
                    }
            })
            .SingleOrDefaultAsync(cancellationToken);

        return Guard.Against.NotFound(request.TaskId, task);
    }
}

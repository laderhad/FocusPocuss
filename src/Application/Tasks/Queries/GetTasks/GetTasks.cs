using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;

namespace FocusPocuss.Application.Tasks.Queries.GetTasks;

[Authorize]
public record GetTasksQuery : IRequest<IReadOnlyList<TaskDto>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetTasksQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!;

        return await _context.TaskItems
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderByDescending(task => task.Created)
            .ThenByDescending(task => task.Id)
            .Select(task => new TaskDto
            {
                Id = task.Id,
                OriginalInput = task.OriginalInput,
                CreatedAt = task.Created
            })
            .ToListAsync(cancellationToken);
    }
}

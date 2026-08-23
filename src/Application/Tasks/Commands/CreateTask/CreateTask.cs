using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.Tasks.Commands.CreateTask;

[Authorize]
public record CreateTaskCommand : IRequest<TaskDto>
{
    public required string OriginalInput { get; init; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateTaskCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = new TaskItem(_user.Id!, request.OriginalInput);

        _context.TaskItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = entity.Id,
            OriginalInput = entity.OriginalInput,
            CreatedAt = entity.Created
        };
    }
}

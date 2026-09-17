using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Common.Security;
using FocusPocuss.Application.Tasks.Planning;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;

[Authorize]
public record CreateTaskStartPlanCommand(int TaskId, string Language) : IRequest<TaskStartPlanDto>;

public class CreateTaskStartPlanCommandHandler : IRequestHandler<CreateTaskStartPlanCommand, TaskStartPlanDto>
{
    private const int MaximumMessageLength = 500;
    private const int MaximumNextActionLength = 500;
    private const int MaximumModelLength = 100;
    private const int MaximumPromptVersionLength = 50;
    private const int MaximumSuggestedDurationMinutes = 60;

    private readonly IApplicationDbContext _context;
    private readonly ITaskStartPlanner _planner;
    private readonly IUser _user;

    public CreateTaskStartPlanCommandHandler(
        IApplicationDbContext context,
        ITaskStartPlanner planner,
        IUser user)
    {
        _context = context;
        _planner = planner;
        _user = user;
    }

    public async Task<TaskStartPlanDto> Handle(
        CreateTaskStartPlanCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .AsNoTracking()
            .Include(item => item.StartPlan)
            .SingleOrDefaultAsync(
                item => item.Id == request.TaskId && item.UserId == _user.Id,
                cancellationToken);

        Guard.Against.NotFound(request.TaskId, task);

        if (task.StartPlan is not null)
        {
            return ToDto(task.StartPlan);
        }

        var result = await _planner.PlanAsync(
            new TaskStartPlanningContext(task.OriginalInput, request.Language),
            cancellationToken);

        EnsureValid(result);

        var plan = new TaskStartPlan(
            task.Id,
            result.Message,
            result.NextAction,
            result.SuggestedDurationMinutes,
            request.Language,
            result.Model,
            result.PromptVersion);

        _context.TaskStartPlans.Add(plan);

        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    private static TaskStartPlanDto ToDto(TaskStartPlan plan)
    {
        return new TaskStartPlanDto
        {
            Id = plan.Id,
            Message = plan.Message,
            NextAction = plan.NextAction,
            SuggestedDurationMinutes = plan.SuggestedDurationMinutes,
            CreatedAt = plan.Created
        };
    }

    private static void EnsureValid(TaskStartPlanResult result)
    {
        if (string.IsNullOrWhiteSpace(result.Message) || result.Message.Length > MaximumMessageLength)
        {
            throw new InvalidOperationException("Task start planner returned an invalid message.");
        }

        if (string.IsNullOrWhiteSpace(result.NextAction) || result.NextAction.Length > MaximumNextActionLength)
        {
            throw new InvalidOperationException("Task start planner returned an invalid next action.");
        }

        if (result.SuggestedDurationMinutes is < 1 or > MaximumSuggestedDurationMinutes)
        {
            throw new InvalidOperationException("Task start planner returned an invalid suggested duration.");
        }

        if (string.IsNullOrWhiteSpace(result.Model) || result.Model.Length > MaximumModelLength)
        {
            throw new InvalidOperationException("Task start planner returned invalid model metadata.");
        }

        if (string.IsNullOrWhiteSpace(result.PromptVersion) || result.PromptVersion.Length > MaximumPromptVersionLength)
        {
            throw new InvalidOperationException("Task start planner returned invalid prompt metadata.");
        }
    }
}

using FocusPocuss.Application.Tasks;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Queries.GetTasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FocusPocuss.Web.Endpoints;

public class Tasks : IEndpointGroup
{
    public static string? RoutePrefix => "/api/tasks";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetTasks);
        groupBuilder.MapPost(CreateTask);
    }

    [EndpointSummary("Get captured tasks")]
    [EndpointDescription("Retrieves the current user's captured tasks, newest first.")]
    public static async Task<Ok<IReadOnlyList<TaskDto>>> GetTasks(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var tasks = await sender.Send(new GetTasksQuery(), cancellationToken);

        return TypedResults.Ok(tasks);
    }

    [EndpointSummary("Capture a task")]
    [EndpointDescription("Stores the current user's original task input without modifying it.")]
    public static async Task<Created<TaskDto>> CreateTask(
        ISender sender,
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await sender.Send(command, cancellationToken);

        return TypedResults.Created((string?)null, task);
    }
}

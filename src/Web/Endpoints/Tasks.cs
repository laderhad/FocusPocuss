using FocusPocuss.Application.Tasks;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Application.Tasks.Queries.GetTaskDetails;
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
        groupBuilder.MapGet(GetTaskDetails, "{id:int}");
        groupBuilder.MapPost(CreateTask);
        groupBuilder.MapPut(CreateTaskStartPlan, "{id:int}/start-plan");
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

        return TypedResults.Created($"/api/tasks/{task.Id}", task);
    }

    [EndpointSummary("Get a captured task")]
    [EndpointDescription("Retrieves a captured task and its start plan for the current user.")]
    public static async Task<Ok<TaskDetailsDto>> GetTaskDetails(
        ISender sender,
        int id,
        CancellationToken cancellationToken)
    {
        var task = await sender.Send(new GetTaskDetailsQuery(id), cancellationToken);

        return TypedResults.Ok(task);
    }

    [EndpointSummary("Create a task start plan")]
    [EndpointDescription("Creates or returns the current user's single persisted start recommendation for a task.")]
    public static async Task<Ok<TaskStartPlanDto>> CreateTaskStartPlan(
        ISender sender,
        int id,
        CreateTaskStartPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await sender.Send(
            new CreateTaskStartPlanCommand(id, request.Language),
            cancellationToken);

        return TypedResults.Ok(plan);
    }
}

public sealed record CreateTaskStartPlanRequest(string Language);

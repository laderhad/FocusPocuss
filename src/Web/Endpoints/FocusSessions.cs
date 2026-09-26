using FocusPocuss.Application.FocusSessions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FocusPocuss.Web.Endpoints;

public class FocusSessions : IEndpointGroup
{
    public static string? RoutePrefix => "/api/focus-sessions";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapPut(StartFocusSession, "active");
        groupBuilder.MapGet(GetFocusSession, "{id:int}");
        groupBuilder.MapPut(CompleteFocusSession, "{id:int}/complete");
    }

    [EndpointSummary("Start or resume a focus session")]
    [EndpointDescription("Creates a focus session from an owned task start plan, or returns the user's existing incomplete session.")]
    public static async Task<Ok<FocusSessionDto>> StartFocusSession(
        ISender sender,
        StartFocusSessionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await sender.Send(
            new StartFocusSessionCommand(request.TaskId, request.TaskStartPlanId),
            cancellationToken);

        return TypedResults.Ok(session);
    }

    [EndpointSummary("Get a focus session")]
    [EndpointDescription("Retrieves an owned focus session, including completed sessions.")]
    public static async Task<Ok<FocusSessionDto>> GetFocusSession(
        ISender sender,
        int id,
        CancellationToken cancellationToken)
    {
        var session = await sender.Send(new GetFocusSessionQuery(id), cancellationToken);

        return TypedResults.Ok(session);
    }

    [EndpointSummary("Complete a focus session")]
    [EndpointDescription("Explicitly completes an owned focus session and returns its persisted state.")]
    public static async Task<Ok<FocusSessionDto>> CompleteFocusSession(
        ISender sender,
        int id,
        CancellationToken cancellationToken)
    {
        var session = await sender.Send(
            new CompleteFocusSessionCommand(id),
            cancellationToken);

        return TypedResults.Ok(session);
    }
}

public sealed record StartFocusSessionRequest(int TaskId, int TaskStartPlanId);

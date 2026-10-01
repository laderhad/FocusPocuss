using FocusPocuss.Application.FocusSessions.Commands.PrepareRecovery;
using FocusPocuss.Application.FocusSessions.Commands.ResolveRecovery;
using FocusPocuss.Application.FocusSessions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.RecordFocusSessionReflection;
using FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSessionHistory;
using FocusPocuss.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FocusPocuss.Web.Endpoints;

public class FocusSessions : IEndpointGroup
{
    public static string? RoutePrefix => "/api/focus-sessions";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapPut(StartFocusSession, "active");
        groupBuilder.MapGet(GetFocusSessionHistory, "history");
        groupBuilder.MapGet(GetFocusSession, "{id:int}");
        groupBuilder.MapPost(ReportDistraction, "{id:int}/distractions");
        groupBuilder.MapPut(PrepareRecovery, "{id:int}/distractions/{distractionId:int}/prepare");
        groupBuilder.MapPut(ResolveRecovery, "{id:int}/distractions/{distractionId:int}/resolve");
        groupBuilder.MapPut(CompleteFocusSession, "{id:int}/complete");
        groupBuilder.MapPut(RecordFocusSessionReflection, "{id:int}/reflection");
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

    [EndpointSummary("Get focus session history")]
    [EndpointDescription("Retrieves the current user's focus sessions, newest first.")]
    public static async Task<Ok<IReadOnlyList<FocusSessionHistoryItemDto>>> GetFocusSessionHistory(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var sessions = await sender.Send(
            new GetFocusSessionHistoryQuery(),
            cancellationToken);

        return TypedResults.Ok(sessions);
    }

    [EndpointSummary("Report a distraction")]
    [EndpointDescription("Records why the user became distracted during an owned active focus session.")]
    public static async Task<Ok<DistractionReportDto>> ReportDistraction(
        ISender sender,
        int id,
        ReportDistractionRequest request,
        CancellationToken cancellationToken)
    {
        var distractionEvent = await sender.Send(
            new ReportDistractionCommand(id, request.Reason, request.Language),
            cancellationToken);

        return TypedResults.Ok(distractionEvent);
    }

    public static async Task<Ok<DistractionReportDto>> PrepareRecovery(
        ISender sender, int id, int distractionId, PrepareRecoveryRequest request, CancellationToken cancellationToken)
        => TypedResults.Ok(await sender.Send(new PrepareRecoveryCommand(id, distractionId,
            request.Choice, request.Clarification), cancellationToken));

    public static async Task<Ok<FocusSessionDto>> ResolveRecovery(
        ISender sender, int id, int distractionId, ResolveRecoveryRequest request, CancellationToken cancellationToken)
        => TypedResults.Ok(await sender.Send(new ResolveRecoveryCommand(id, distractionId,
            request.Resolution, request.Thought), cancellationToken));

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

    [EndpointSummary("Record a focus session reflection")]
    [EndpointDescription("Records or updates the reflection for an owned completed focus session.")]
    public static async Task<Ok<FocusSessionDto>> RecordFocusSessionReflection(
        ISender sender,
        int id,
        RecordFocusSessionReflectionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await sender.Send(
            new RecordFocusSessionReflectionCommand(id, request.Reflection),
            cancellationToken);

        return TypedResults.Ok(session);
    }
}

public sealed record StartFocusSessionRequest(int TaskId, int TaskStartPlanId);

public sealed record ReportDistractionRequest(DistractionReason Reason, string Language = "en");

public sealed record PrepareRecoveryRequest(RecoveryChoice? Choice, string? Clarification);

public sealed record ResolveRecoveryRequest(RecoveryResolution Resolution, string? Thought);

public sealed record RecordFocusSessionReflectionRequest(FocusSessionReflection Reflection);

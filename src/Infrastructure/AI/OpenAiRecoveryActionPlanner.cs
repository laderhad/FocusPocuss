using System.Text.Json;
using System.Text.Json.Serialization;
using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Infrastructure.AI.Prompts;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ClientModel;

namespace FocusPocuss.Infrastructure.AI;

public sealed class OpenAiRecoveryActionPlanner(
    Func<IChatClient> clientFactory,
    ILogger<OpenAiRecoveryActionPlanner> logger) : IRecoveryActionPlanner
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter<RecoveryActionStatus>(allowIntegerValues: false) }
    };

    public async Task<RecoveryActionResult> TransformAsync(RecoveryActionContext context, CancellationToken cancellationToken)
    {
        var prompt = RecoveryActionPrompt.Create(context.Transformation, context.Language);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        try
        {
            var response = await clientFactory().GetResponseAsync(
                [new ChatMessage(new ChatRole("developer"), prompt),
                 new ChatMessage(ChatRole.User, JsonSerializer.Serialize(new
                 {
                     context.CurrentAction, context.Clarification
                 }, JsonOptions))],
                new ChatOptions
                {
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<TransformationResponse>(JsonOptions,
                        schemaName: "recovery_action")
                }, timeout.Token);
            var result = JsonSerializer.Deserialize<TransformationResponse>(response.Text, JsonOptions);
            if (result?.Status == RecoveryActionStatus.Ready && !string.IsNullOrWhiteSpace(result.Action)
                && result.Action.Trim().Length <= 500
                && !string.Equals(result.Action.Trim(), context.CurrentAction.Trim(), StringComparison.OrdinalIgnoreCase))
                return new(RecoveryActionStatus.Ready, result.Action.Trim());
            if (result?.Status == RecoveryActionStatus.NeedsClarification && result.Action is null && context.Clarification is null)
                return new(RecoveryActionStatus.NeedsClarification);
            return new(RecoveryActionStatus.Unavailable);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Recovery transformation timed out.");
            return new(RecoveryActionStatus.Unavailable);
        }
        catch (Exception exception) when (exception is JsonException or HttpRequestException
            or ClientResultException or OptionsValidationException)
        {
            // Do not log prompts, user input, response content, or provider exception details.
            logger.LogWarning("Recovery transformation unavailable ({FailureType}).", exception.GetType().Name);
            return new(RecoveryActionStatus.Unavailable);
        }
    }

    private sealed class TransformationResponse
    {
        [JsonRequired] public required RecoveryActionStatus Status { get; init; }
        [JsonRequired] public required string? Action { get; init; }
    }
}

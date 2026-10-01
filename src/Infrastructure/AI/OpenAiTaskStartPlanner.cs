using System.Text.Json;
using System.Text.Json.Serialization;
using FocusPocuss.Application.Tasks.Planning;
using FocusPocuss.Infrastructure.AI.Prompts;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace FocusPocuss.Infrastructure.AI;

public sealed class OpenAiTaskStartPlanner : ITaskStartPlanner
{
    public const string PromptVersion = TaskStartPlannerPrompt.Version;

    private static readonly ChatRole DeveloperRole = new("developer");

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    private readonly IChatClient _chatClient;
    private readonly string _configuredModel;

    public OpenAiTaskStartPlanner(
        IChatClient chatClient,
        IOptions<OpenAiOptions> options)
    {
        _chatClient = chatClient;
        _configuredModel = options.Value.Model;
    }

    public async Task<TaskStartPlanResult> PlanAsync(
        TaskStartPlanningContext context,
        CancellationToken cancellationToken)
    {
        var response = await _chatClient.GetResponseAsync(
            [
                new ChatMessage(
                    DeveloperRole,
                    TaskStartPlannerPrompt.Create(context.Language)),
                new ChatMessage(
                    ChatRole.User,
                    context.OriginalInput)
            ],
            new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.ForJsonSchema<PlannerResponse>(
                    JsonOptions,
                    schemaName: "task_start_plan",
                    schemaDescription:
                        "One small but meaningful work unit that helps the user start the captured task.")
            },
            cancellationToken);

        PlannerResponse result;

        try
        {
            result = JsonSerializer.Deserialize<PlannerResponse>(
                         response.Text,
                         JsonOptions)
                     ?? throw new InvalidOperationException(
                         "OpenAI returned an empty task start plan.");
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "OpenAI returned an invalid task start plan.",
                exception);
        }

        ValidateResponse(result);

        return new TaskStartPlanResult(
            result.Message.Trim(),
            result.NextAction.Trim(),
            result.SuggestedDurationMinutes,
            response.ModelId ?? _configuredModel,
            PromptVersion);
    }

    private static void ValidateResponse(PlannerResponse result)
    {
        if (string.IsNullOrWhiteSpace(result.Message))
        {
            throw new InvalidOperationException(
                "OpenAI returned an empty task start message.");
        }

        if (string.IsNullOrWhiteSpace(result.NextAction))
        {
            throw new InvalidOperationException(
                "OpenAI returned an empty next action.");
        }

        if (result.SuggestedDurationMinutes is < TaskStartPlanLimits.MinimumSuggestedDurationMinutes
            or > TaskStartPlanLimits.MaximumSuggestedDurationMinutes)
        {
            throw new InvalidOperationException(
                $"OpenAI returned an invalid suggested duration: " +
                $"{result.SuggestedDurationMinutes}. Expected " +
                $"{TaskStartPlanLimits.MinimumSuggestedDurationMinutes}-{TaskStartPlanLimits.MaximumSuggestedDurationMinutes} minutes.");
        }
    }

    private sealed class PlannerResponse
    {
        [JsonRequired]
        public required string Message { get; init; }

        [JsonRequired]
        public required string NextAction { get; init; }

        [JsonRequired]
        public int SuggestedDurationMinutes { get; init; }
    }
}
namespace FocusPocuss.Infrastructure.AI;

public sealed class OpenAiOptions
{
    public const string SectionName = "AI:OpenAI";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;
}

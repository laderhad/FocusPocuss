namespace FocusPocuss.Domain.Entities;

public class DistractionEvent : BaseEntity
{
    private DistractionEvent()
    {
    }

    public DistractionEvent(
        int focusSessionId,
        DistractionReason reason,
        DateTimeOffset occurredAtUtc)
    {
        if (!Enum.IsDefined(reason))
        {
            throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unsupported distraction reason.");
        }

        FocusSessionId = focusSessionId;
        Reason = reason;
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
    }

    public int FocusSessionId { get; private set; }

    public DistractionReason Reason { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public RecoveryInterventionType? InterventionType { get; private set; }
    public string? StrategyVersion { get; private set; }
    public string? ActionAtDistraction { get; private set; }
    public string? ProposedAction { get; private set; }
    public string? Language { get; private set; }
    public RecoveryRequirement? Requirement { get; private set; }
    public RecoveryChoice? Choice { get; private set; }
    public bool ClarificationUsed { get; private set; }
    public string? ParkedThought { get; private set; }
    public RecoveryResolution? Resolution { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }

    public void BeginRecovery(RecoveryInterventionType type, string version, string action,
        string language, RecoveryRequirement requirement)
    {
        if (InterventionType is not null) throw new InvalidOperationException("Recovery already exists.");
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        if (!Enum.IsDefined(type) || !Enum.IsDefined(requirement)) throw new ArgumentOutOfRangeException(nameof(type));
        if (language is not ("tr" or "en")) throw new ArgumentOutOfRangeException(nameof(language));
        InterventionType = type;
        StrategyVersion = version;
        ActionAtDistraction = action;
        Language = language;
        Requirement = requirement;
        ProposedAction = requirement == RecoveryRequirement.ActionTransformation ? null : action;
    }

    public void Prepare(string? action, RecoveryRequirement requirement)
    {
        EnsurePending();
        if (requirement is not (RecoveryRequirement.None or RecoveryRequirement.NeedsClarification or RecoveryRequirement.Unavailable))
            throw new ArgumentOutOfRangeException(nameof(requirement));
        if (requirement == RecoveryRequirement.None && (string.IsNullOrWhiteSpace(action) || action.Trim().Length > 500))
            throw new ArgumentException("A bounded action is required.", nameof(action));
        Requirement = requirement;
        ProposedAction = requirement == RecoveryRequirement.None ? action!.Trim() : null;
    }

    public void UseClarification()
    {
        EnsurePending();
        if (ClarificationUsed || Requirement != RecoveryRequirement.NeedsClarification)
            throw new InvalidOperationException("Clarification is not available.");
        ClarificationUsed = true;
    }

    public void SelectChoice(RecoveryChoice choice)
    {
        EnsurePending();
        if (InterventionType != RecoveryInterventionType.RecoveryChoice || !Enum.IsDefined(choice))
            throw new InvalidOperationException("This intervention does not support that choice.");
        Choice = choice;
        Requirement = choice == RecoveryChoice.ContinueWithSmallerAction
            ? RecoveryRequirement.ActionTransformation : RecoveryRequirement.None;
        ProposedAction = choice == RecoveryChoice.TakeShortReset ? ActionAtDistraction : null;
    }

    public void Resolve(RecoveryResolution resolution, DateTimeOffset resolvedAtUtc, string? thought = null)
    {
        EnsurePending();
        if (!Enum.IsDefined(resolution)) throw new ArgumentOutOfRangeException(nameof(resolution));
        if (resolution == RecoveryResolution.EndSession && InterventionType != RecoveryInterventionType.RecoveryChoice)
            throw new InvalidOperationException("Ending through recovery requires an explicit recovery choice.");
        if (resolution == RecoveryResolution.ReturnToFocus)
        {
            if (Requirement == RecoveryRequirement.ThoughtCapture)
            {
                if (string.IsNullOrWhiteSpace(thought) || thought.Trim().Length > 1000)
                    throw new ArgumentException("A thought of up to 1000 characters is required.", nameof(thought));
                ParkedThought = thought.Trim();
            }
            else if (Requirement != RecoveryRequirement.None || string.IsNullOrWhiteSpace(ProposedAction))
                throw new InvalidOperationException("Recovery is not ready to return.");
        }
        if (resolution == RecoveryResolution.EndSession) Choice = RecoveryChoice.EndSession;
        Resolution = resolution;
        ResolvedAtUtc = resolvedAtUtc.ToUniversalTime();
    }

    private void EnsurePending()
    {
        if (InterventionType is null || ResolvedAtUtc is not null)
            throw new InvalidOperationException("No pending recovery exists.");
    }

    public FocusSession FocusSession { get; private set; } = null!;
}

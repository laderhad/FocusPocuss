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

    public FocusSession FocusSession { get; private set; } = null!;
}

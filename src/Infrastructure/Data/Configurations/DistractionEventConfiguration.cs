using FocusPocuss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FocusPocuss.Infrastructure.Data.Configurations;

public class DistractionEventConfiguration : IEntityTypeConfiguration<DistractionEvent>
{
    public void Configure(EntityTypeBuilder<DistractionEvent> builder)
    {
        builder.Property(x => x.StrategyVersion).HasMaxLength(50);
        builder.Property(x => x.ActionAtDistraction).HasMaxLength(500);
        builder.Property(x => x.ProposedAction).HasMaxLength(500);
        builder.Property(x => x.Language).HasMaxLength(2);
        builder.Property(x => x.ParkedThought).HasMaxLength(1000);
        builder.HasIndex(x => x.FocusSessionId).IsUnique()
            .HasFilter("\"InterventionType\" IS NOT NULL AND \"ResolvedAtUtc\" IS NULL")
            .HasDatabaseName("IX_DistractionEvents_PendingRecovery");

        builder.Property(distraction => distraction.Reason)
            .IsRequired();

        builder.Property(distraction => distraction.OccurredAtUtc)
            .IsRequired();

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_DistractionEvents_Reason",
            "\"Reason\" BETWEEN 1 AND 6"));

        builder.HasOne(distraction => distraction.FocusSession)
            .WithMany(session => session.DistractionEvents)
            .HasForeignKey(distraction => distraction.FocusSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

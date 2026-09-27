using FocusPocuss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FocusPocuss.Infrastructure.Data.Configurations;

public class DistractionEventConfiguration : IEntityTypeConfiguration<DistractionEvent>
{
    public void Configure(EntityTypeBuilder<DistractionEvent> builder)
    {
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

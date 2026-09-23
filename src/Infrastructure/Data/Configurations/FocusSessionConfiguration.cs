using FocusPocuss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FocusPocuss.Infrastructure.Data.Configurations;

public class FocusSessionConfiguration : IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> builder)
    {
        builder.Property(session => session.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(session => session.Action)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(session => session.UserId)
            .IsUnique()
            .HasFilter("\"CompletedAtUtc\" IS NULL")
            .HasDatabaseName("IX_FocusSessions_UserId_Active");

        builder.HasOne(session => session.TaskItem)
            .WithMany()
            .HasForeignKey(session => session.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(session => session.TaskStartPlan)
            .WithMany()
            .HasForeignKey(session => session.TaskStartPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

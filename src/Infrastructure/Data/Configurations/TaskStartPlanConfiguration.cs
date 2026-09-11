using FocusPocuss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FocusPocuss.Infrastructure.Data.Configurations;

public class TaskStartPlanConfiguration : IEntityTypeConfiguration<TaskStartPlan>
{
    public void Configure(EntityTypeBuilder<TaskStartPlan> builder)
    {
        builder.Property(plan => plan.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(plan => plan.NextAction)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(plan => plan.Language)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(plan => plan.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(plan => plan.PromptVersion)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(plan => plan.TaskItem)
            .WithOne(task => task.StartPlan)
            .HasForeignKey<TaskStartPlan>(plan => plan.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

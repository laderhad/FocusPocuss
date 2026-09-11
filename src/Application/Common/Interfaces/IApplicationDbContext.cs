using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<TaskItem> TaskItems { get; }

    DbSet<TaskStartPlan> TaskStartPlans { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

import { useTranslation } from 'react-i18next';
import { TaskCaptureForm, TaskList, useTasks } from '../features/tasks';

export function TasksPage() {
  const { t } = useTranslation();
  const tasksQuery = useTasks();

  return (
    <section className="task-capture-page">
      <header className="task-capture-header">
        <h1>{t('tasks.title')}</h1>
        <p>{t('tasks.subtitle')}</p>
      </header>

      <TaskCaptureForm />

      <section className="task-history" aria-labelledby="task-history-title">
        <h2 id="task-history-title">{t('tasks.history.title')}</h2>
        <TaskList
          tasks={tasksQuery.data ?? []}
          isLoading={tasksQuery.isLoading}
          isError={tasksQuery.isError}
          onRetry={() => void tasksQuery.refetch()}
        />
      </section>
    </section>
  );
}

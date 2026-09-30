import { useTranslation } from 'react-i18next';
import { TaskCaptureForm, TaskList, useTasks } from '../features/tasks';
import launchpadContour from '../shared/assets/launchpad-contour.svg?raw';

export function TasksPage() {
  const { t } = useTranslation();
  const tasksQuery = useTasks();

  return (
    <section className="task-capture-page" aria-labelledby="launchpad-title">
      {/* Static local artwork, inlined so its colors inherit the active theme. */}
      <div
        className="launchpad-contour"
        aria-hidden="true"
        dangerouslySetInnerHTML={{ __html: launchpadContour }}
      />
      <div className="launchpad-content">
        <header className="task-capture-header">
          <h1 id="launchpad-title">{t('tasks.title')}</h1>
        </header>

        <TaskCaptureForm />

        <section className="task-history" aria-labelledby="task-history-title">
          <h2 id="task-history-title">
            {t(tasksQuery.data?.length ? 'tasks.history.continuation' : 'tasks.history.title')}
          </h2>
          <TaskList
            tasks={tasksQuery.data ?? []}
            isLoading={tasksQuery.isLoading}
            isError={tasksQuery.isError}
            onRetry={() => void tasksQuery.refetch()}
          />
        </section>
      </div>
    </section>
  );
}

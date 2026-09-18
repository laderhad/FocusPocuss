import { ArrowLeft, RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import { TaskStartPlanPanel, useTaskDetails } from '../features/tasks';

export function TaskDetailsPage() {
  const { t } = useTranslation();
  const { taskId } = useParams();
  const parsedTaskId = Number(taskId);
  const isValidTaskId = Number.isInteger(parsedTaskId) && parsedTaskId > 0;
  const taskQuery = useTaskDetails(parsedTaskId);

  if (!isValidTaskId) {
    return <TaskDetailsError message={t('tasks.detail.notFound')} />;
  }

  if (taskQuery.isLoading) {
    return <p className="task-detail-state" aria-live="polite">{t('tasks.detail.loading')}</p>;
  }

  if (taskQuery.isError || !taskQuery.data) {
    return (
      <TaskDetailsError
        message={t('tasks.detail.loadError')}
        onRetry={() => void taskQuery.refetch()}
      />
    );
  }

  return (
    <section className="task-detail-page">
      <Link to="/tasks" className="task-detail-back">
        <ArrowLeft size={18} aria-hidden="true" />
        {t('tasks.detail.back')}
      </Link>

      <header className="task-detail-header">
        <p>{t('tasks.detail.capturedTask')}</p>
        <h1>{taskQuery.data.originalInput}</h1>
      </header>

      <TaskStartPlanPanel
        taskId={parsedTaskId}
        existingPlan={taskQuery.data.startPlan}
      />
    </section>
  );
}

interface TaskDetailsErrorProps {
  message: string;
  onRetry?: () => void;
}

function TaskDetailsError({ message, onRetry }: TaskDetailsErrorProps) {
  const { t } = useTranslation();

  return (
    <section className="task-detail-state" role="alert">
      <p>{message}</p>
      {onRetry && (
        <button type="button" className="secondary outline" onClick={onRetry}>
          <RefreshCw size={18} aria-hidden="true" />
          {t('tasks.detail.retry')}
        </button>
      )}
      <Link to="/tasks">{t('tasks.detail.back')}</Link>
    </section>
  );
}

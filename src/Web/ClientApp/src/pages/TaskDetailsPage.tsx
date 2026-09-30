import { ArrowLeft, RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import { TaskStartPlanPanel, useTaskDetails } from '../features/tasks';
import gatheringLines from '../shared/assets/gathering-lines.svg?raw';

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
    return (
      <section className="task-detail-state">
        <h1>{t('tasks.detail.title')}</h1>
        <p aria-live="polite">{t('tasks.detail.loading')}</p>
      </section>
    );
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
    <section className="task-detail-page" aria-labelledby="task-detail-title">
      <Link to="/tasks" className="task-detail-back">
        <ArrowLeft size={18} aria-hidden="true" />
        {t('tasks.detail.back')}
      </Link>

      <header className="task-detail-header">
        <h1 id="task-detail-title">{t('tasks.detail.title')}</h1>
        <p className="task-detail-context-label">{t('tasks.detail.capturedTask')}</p>
        <p className="task-detail-original-input">{taskQuery.data.originalInput}</p>
      </header>

      {/* Trusted local SVG; inline colors follow the existing theme tokens. */}
      <div
        className="task-preparation-artwork"
        aria-hidden="true"
        dangerouslySetInnerHTML={{ __html: gatheringLines }}
      />

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
      <h1>{t('tasks.detail.title')}</h1>
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

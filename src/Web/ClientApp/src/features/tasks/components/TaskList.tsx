import { ArrowRight, RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import type { TaskDto } from '../api/tasksApi';

interface TaskListProps {
  tasks: TaskDto[];
  isLoading: boolean;
  isError: boolean;
  onRetry: () => void;
}

export function TaskList({ tasks, isLoading, isError, onRetry }: TaskListProps) {
  const { t } = useTranslation();

  if (isLoading) {
    return <p className="task-history-state" aria-live="polite">{t('tasks.history.loading')}</p>;
  }

  if (isError && tasks.length === 0) {
    return (
      <div className="task-history-state" role="alert">
        <p>{t('tasks.history.loadError')}</p>
        <button type="button" className="secondary outline" onClick={onRetry}>
          <RefreshCw size={18} aria-hidden="true" />
          {t('tasks.history.retry')}
        </button>
      </div>
    );
  }

  if (tasks.length === 0) {
    return <p className="task-history-state">{t('tasks.history.empty')}</p>;
  }

  // The API already orders tasks newest first. Feature the first without
  // implying a session status or a next action that the task DTO doesn't carry.
  const [featuredTask, ...remainingTasks] = tasks;

  return (
    <div>
      <div className="task-continuation">
        <p className="task-continuation-title" id="task-continuation-title">
          {featuredTask.originalInput}
        </p>
        {featuredTask.id !== undefined && (
          <Link
            to={`/tasks/${featuredTask.id}`}
            className="task-continuation-link"
            aria-describedby="task-continuation-title"
          >
            {t('tasks.history.continue')}
            <ArrowRight size={16} aria-hidden="true" />
          </Link>
        )}
      </div>
      {remainingTasks.length > 0 && (
        <details className="task-history-disclosure">
          <summary>{t('tasks.history.remaining', { count: remainingTasks.length })}</summary>
          <ol className="task-history-list">
            {remainingTasks.map((task) => (
              <li key={task.id} className="task-history-item">
                {task.id === undefined
                  ? task.originalInput
                  : <Link to={`/tasks/${task.id}`}>{task.originalInput}</Link>}
              </li>
            ))}
          </ol>
        </details>
      )}
    </div>
  );
}

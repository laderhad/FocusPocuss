import { RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
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

  return (
    <ol className="task-history-list">
      {tasks.map((task) => (
        <li key={task.id} className="task-history-item">
          {task.originalInput}
        </li>
      ))}
    </ol>
  );
}

import { ArrowRight } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';
import { useStartFocusSession } from '../api/focusQueries';

interface StartFocusSessionButtonProps {
  taskId: number;
  taskStartPlanId: number;
}

export function StartFocusSessionButton({
  taskId,
  taskStartPlanId,
}: StartFocusSessionButtonProps) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const startSession = useStartFocusSession();
  const existingSession = startSession.isSuccess
    && startSession.variables?.taskId === taskId
    && startSession.variables?.taskStartPlanId === taskStartPlanId
    && (startSession.data.taskId !== taskId || startSession.data.taskStartPlanId !== taskStartPlanId)
    ? startSession.data
    : undefined;

  const handleStart = () => {
    startSession.mutate(
      { taskId, taskStartPlanId },
      {
        onSuccess: (session) => {
          if (session.id !== undefined
            && session.taskId === taskId
            && session.taskStartPlanId === taskStartPlanId) {
            navigate(`/focus/${session.id}`);
          }
        },
      },
    );
  };

  return (
    <div className="focus-start-control">
      <button
        type="button"
        className="focus-start-button"
        disabled={startSession.isPending}
        aria-describedby={startSession.isError ? 'focus-start-error' : undefined}
        onClick={handleStart}
      >
        <span aria-live="polite">
          {startSession.isPending ? t('focus.start.starting') : t('focus.start.action')}
        </span>
        <ArrowRight size={18} aria-hidden="true" />
      </button>
      {existingSession && (
        <div className="focus-existing-session">
          <div role="status">
            <p className="focus-existing-session-title">{t('focus.start.existingTitle')}</p>
            <p>{t('focus.start.existingDescription')}</p>
            <p className="focus-existing-session-action">{existingSession.action}</p>
          </div>
          <Link to={`/focus/${existingSession.id}`}>
            {t('focus.start.resumeExisting')}
            <ArrowRight size={16} aria-hidden="true" />
          </Link>
        </div>
      )}
      {startSession.isError && (
        <p id="focus-start-error" className="focus-start-error" role="alert">
          {t('focus.start.error')}
        </p>
      )}
    </div>
  );
}

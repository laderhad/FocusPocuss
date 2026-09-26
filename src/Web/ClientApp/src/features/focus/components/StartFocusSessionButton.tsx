import { Play } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
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

  const handleStart = () => {
    startSession.mutate(
      { taskId, taskStartPlanId },
      {
        onSuccess: (session) => {
          if (session.id !== undefined) {
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
        onClick={handleStart}
      >
        <Play size={18} aria-hidden="true" />
        {startSession.isPending ? t('focus.start.starting') : t('focus.start.action')}
      </button>
      {startSession.isError && (
        <p className="focus-start-error" role="alert">
          {t('focus.start.error')}
        </p>
      )}
    </div>
  );
}

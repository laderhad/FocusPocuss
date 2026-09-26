import { Check, Clock3 } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import type { FocusSessionDto } from '../api/focusApi';
import { useRemainingSeconds } from '../model/useRemainingSeconds';

interface FocusSessionViewProps {
  session: FocusSessionDto;
  isCompleting: boolean;
  completionFailed: boolean;
  onComplete: () => void;
}

function formatRemainingTime(totalSeconds: number) {
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
}

export function FocusSessionView({
  session,
  isCompleting,
  completionFailed,
  onComplete,
}: FocusSessionViewProps) {
  const { t } = useTranslation();
  const isCompleted = session.completedAtUtc !== undefined;
  const remainingSeconds = useRemainingSeconds(
    session.startedAtUtc,
    session.plannedDurationMinutes,
    !isCompleted,
  );

  if (isCompleted) {
    const taskPath = session.taskId ? `/tasks/${session.taskId}` : '/tasks';

    return (
      <section className="focus-session-result" aria-labelledby="focus-completed-title">
        <Check size={32} aria-hidden="true" />
        <h1 id="focus-completed-title">{t('focus.session.completed')}</h1>
        <p>{t('focus.session.completedMessage')}</p>
        <Link to={taskPath}>{t('focus.session.backToTask')}</Link>
      </section>
    );
  }

  return (
    <section className="focus-session" aria-labelledby="focus-action">
      <p className="focus-session-label">{t('focus.session.label')}</p>
      <h1 id="focus-action">{session.action}</h1>

      <div className="focus-session-timer">
        <p>
          <Clock3 size={18} aria-hidden="true" />
          {t('focus.session.remaining')}
        </p>
        <time dateTime={`PT${remainingSeconds}S`}>
          {formatRemainingTime(remainingSeconds)}
        </time>
      </div>

      {remainingSeconds === 0 && (
        <p className="focus-session-expired" role="status">
          {t('focus.session.expired')}
        </p>
      )}

      <button type="button" disabled={isCompleting} onClick={onComplete}>
        <Check size={18} aria-hidden="true" />
        {isCompleting ? t('focus.session.completing') : t('focus.session.complete')}
      </button>

      {completionFailed && (
        <p className="focus-session-error" role="alert">
          {t('focus.session.completeError')}
        </p>
      )}
    </section>
  );
}

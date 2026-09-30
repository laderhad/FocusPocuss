import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import type { FocusSessionDto } from '../api/focusApi';
import { useRemainingSeconds } from '../model/useRemainingSeconds';
import { DistractionReporter } from './DistractionReporter';
import { FocusSessionReflectionPrompt } from './FocusSessionReflectionPrompt';
import { FocusArtwork } from './FocusArtwork';
import { CompletionHeading } from './CompletionHeading';

interface FocusSessionViewProps {
  sessionId: number;
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
  sessionId,
  session,
  isCompleting,
  completionFailed,
  onComplete,
}: FocusSessionViewProps) {
  const { t } = useTranslation();
  const [isTimerVisible, setIsTimerVisible] = useState(true);
  const [skippedReflectionSessionId, setSkippedReflectionSessionId] =
    useState<number>();
  const isCompleted = session.completedAtUtc !== undefined;
  const remainingSeconds = useRemainingSeconds(
    session.startedAtUtc,
    session.plannedDurationMinutes,
    !isCompleted,
  );

  if (
    isCompleted &&
    (session.reflection === undefined || session.reflection === null) &&
    skippedReflectionSessionId !== sessionId
  ) {
    return (
      <FocusSessionReflectionPrompt
        key={sessionId}
        sessionId={sessionId}
        onSkip={() => setSkippedReflectionSessionId(sessionId)}
      />
    );
  }

  if (isCompleted) {
    const taskPath = session.taskId ? `/tasks/${session.taskId}` : '/tasks';

    return (
      <section className="focus-session-result" aria-labelledby="focus-completed-title">
        <CompletionHeading />
        <div className="focus-result-actions">
          <Link to={taskPath} role="button">{t('focus.session.backToTask')}</Link>
          <Link to="/tasks">{t('focus.session.backToTasks')}</Link>
        </div>
      </section>
    );
  }

  return (
    <section className="focus-session" aria-labelledby="focus-action">
      <p className="focus-session-label">{t('focus.session.label')}</p>
      <h1 id="focus-action">{session.action}</h1>

      <div className="focus-session-timer">
        <FocusArtwork variant="focus" />
        <div className="focus-timer-content">
          <div id="focus-timer-value" className="focus-timer-value">
            {isTimerVisible && (
              <time
                id="focus-remaining-time"
                dateTime={`PT${remainingSeconds}S`}
                aria-label={`${t('focus.session.remaining')}: ${formatRemainingTime(remainingSeconds)}`}
              >
                {formatRemainingTime(remainingSeconds)}
              </time>
            )}
          </div>
          <button
            type="button"
            className="focus-timer-toggle"
            aria-controls="focus-timer-value"
            aria-expanded={isTimerVisible}
            onClick={() => setIsTimerVisible(visible => !visible)}
          >
            {t(isTimerVisible ? 'focus.session.hideTimer' : 'focus.session.showTimer')}
          </button>
        </div>
      </div>

      {remainingSeconds === 0 && (
        <p className="focus-session-expired" role="status">
          {t('focus.session.expired')}
        </p>
      )}

      <div className="focus-session-actions">
        <button className="focus-complete-button" type="button" disabled={isCompleting} onClick={onComplete}>
          {isCompleting ? t('focus.session.completing') : t('focus.session.complete')}
        </button>
        <DistractionReporter sessionId={sessionId} />
      </div>

      {completionFailed && (
        <p className="focus-session-error" role="alert">
          {t('focus.session.completeError')}
        </p>
      )}
    </section>
  );
}

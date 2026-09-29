import { RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import {
  FocusSessionReflection,
  type FocusSessionHistoryItemDto,
} from '../api/focusApi';

const reflectionTranslationKeys: Record<FocusSessionReflection, string> = {
  [FocusSessionReflection.FocusedWell]: 'focus.reflection.options.focusedWell',
  [FocusSessionReflection.SomeDifficulty]:
    'focus.reflection.options.someDifficulty',
  [FocusSessionReflection.SignificantDifficulty]:
    'focus.reflection.options.significantDifficulty',
};

interface FocusSessionHistoryListProps {
  sessions: FocusSessionHistoryItemDto[];
  isLoading: boolean;
  isError: boolean;
  onRetry: () => void;
}

export function FocusSessionHistoryList({
  sessions,
  isLoading,
  isError,
  onRetry,
}: FocusSessionHistoryListProps) {
  const { t, i18n } = useTranslation();
  const dateFormatter = new Intl.DateTimeFormat(
    i18n.resolvedLanguage ?? i18n.language,
    {
      dateStyle: 'medium',
      timeStyle: 'short',
    },
  );

  if (isLoading) {
    return (
      <p className="session-history-state" aria-live="polite">
        {t('focus.history.loading')}
      </p>
    );
  }

  if (isError && sessions.length === 0) {
    return (
      <div className="session-history-state" role="alert">
        <p>{t('focus.history.loadError')}</p>
        <button type="button" className="secondary outline" onClick={onRetry}>
          <RefreshCw size={18} aria-hidden="true" />
          {t('focus.history.retry')}
        </button>
      </div>
    );
  }

  if (sessions.length === 0) {
    return <p className="session-history-state">{t('focus.history.empty')}</p>;
  }

  return (
    <ol className="session-history-list">
      {sessions.map((session, index) => {
        const isCompleted = session.completedAtUtc !== undefined
          && session.completedAtUtc !== null;
        const reflectionTranslationKey = session.reflection === undefined
          || session.reflection === null
          ? undefined
          : reflectionTranslationKeys[session.reflection];

        return (
          <li
            key={session.id ?? `${session.action}-${index}`}
            className="session-history-item"
          >
            <div className="session-history-item-header">
              <h2>
                {session.taskId === undefined
                  ? session.action
                  : <Link to={`/tasks/${session.taskId}`}>{session.action}</Link>}
              </h2>
              <span className="session-history-status">
                {t(isCompleted
                  ? 'focus.history.completed'
                  : 'focus.history.incomplete')}
              </span>
            </div>

            <div className="session-history-meta">
              {session.startedAtUtc && (
                <time dateTime={session.startedAtUtc.toISOString()}>
                  {dateFormatter.format(session.startedAtUtc)}
                </time>
              )}
              {session.plannedDurationMinutes !== undefined && (
                <span>
                  {t('focus.history.duration', {
                    count: session.plannedDurationMinutes,
                  })}
                </span>
              )}
            </div>

            {reflectionTranslationKey && (
              <p className="session-history-reflection">
                {t('focus.history.reflection', {
                  value: t(reflectionTranslationKey),
                })}
              </p>
            )}
          </li>
        );
      })}
    </ol>
  );
}

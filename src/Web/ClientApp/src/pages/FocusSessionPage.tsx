import { RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import { NavMenu } from '../components/NavMenu';
import {
  FocusSessionView,
  useCompleteFocusSession,
  useFocusSession,
} from '../features/focus';

export function FocusSessionPage() {
  const { t } = useTranslation();
  const { sessionId } = useParams();
  const parsedSessionId = Number(sessionId);
  const isValidSessionId = Number.isInteger(parsedSessionId) && parsedSessionId > 0;
  const sessionQuery = useFocusSession(parsedSessionId);
  const completeSession = useCompleteFocusSession(parsedSessionId);

  if (!isValidSessionId) {
    return <FocusSessionError message={t('focus.session.notFound')} />;
  }

  if (sessionQuery.isLoading) {
    return (
      <p className="focus-session-state" aria-live="polite">
        {t('focus.session.loading')}
      </p>
    );
  }

  if (sessionQuery.isError || !sessionQuery.data) {
    return (
      <FocusSessionError
        message={t('focus.session.loadError')}
        onRetry={() => void sessionQuery.refetch()}
      />
    );
  }

  return (
    <div className={(sessionQuery.data.completedAtUtc != null || sessionQuery.data.endedEarlyAtUtc != null) ? 'focus-completed-shell' : undefined}>
      {(sessionQuery.data.completedAtUtc != null || sessionQuery.data.endedEarlyAtUtc != null) && <NavMenu />}
      <div className="focus-session-page">
        <FocusSessionView
          sessionId={parsedSessionId}
          session={sessionQuery.data}
          isCompleting={completeSession.isPending}
          completionFailed={completeSession.isError}
          onComplete={() => completeSession.mutate()}
        />
      </div>
    </div>
  );
}

interface FocusSessionErrorProps {
  message: string;
  onRetry?: () => void;
}

function FocusSessionError({ message, onRetry }: FocusSessionErrorProps) {
  const { t } = useTranslation();

  return (
    <section className="focus-session-state" role="alert">
      <p>{message}</p>
      {onRetry && (
        <button type="button" className="secondary outline" onClick={onRetry}>
          <RefreshCw size={18} aria-hidden="true" />
          {t('focus.session.retry')}
        </button>
      )}
      <Link to="/tasks">{t('focus.session.backToTasks')}</Link>
    </section>
  );
}

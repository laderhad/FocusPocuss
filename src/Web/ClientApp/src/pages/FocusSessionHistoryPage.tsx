import { useTranslation } from 'react-i18next';
import {
  FocusSessionHistoryList,
  useFocusSessionHistory,
  WeeklyFocusSummary,
} from '../features/focus';

export function FocusSessionHistoryPage() {
  const { t } = useTranslation();
  const historyQuery = useFocusSessionHistory();

  return (
    <section className="session-history-page" aria-labelledby="session-history-title">
      <header>
        <h1 id="session-history-title">{t('focus.history.title')}</h1>
        <p>{t('focus.history.subtitle')}</p>
      </header>

      {historyQuery.data && !historyQuery.isError && (
        <WeeklyFocusSummary sessions={historyQuery.data} />
      )}
      <h2 className="session-history-ledger-title">{t('focus.history.recent')}</h2>
      <FocusSessionHistoryList
        sessions={historyQuery.data ?? []}
        isLoading={historyQuery.isLoading}
        isError={historyQuery.isError}
        onRetry={() => void historyQuery.refetch()}
      />
    </section>
  );
}

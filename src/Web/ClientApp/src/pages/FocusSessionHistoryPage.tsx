import { useTranslation } from 'react-i18next';
import {
  FocusSessionHistoryList,
  useFocusSessionHistory,
} from '../features/focus';

export function FocusSessionHistoryPage() {
  const { t } = useTranslation();
  const historyQuery = useFocusSessionHistory();

  return (
    <section className="session-history-page" aria-labelledby="session-history-title">
      <header>
        <h1 id="session-history-title">{t('focus.history.title')}</h1>
      </header>

      <FocusSessionHistoryList
        sessions={historyQuery.data ?? []}
        isLoading={historyQuery.isLoading}
        isError={historyQuery.isError}
        onRetry={() => void historyQuery.refetch()}
      />
    </section>
  );
}

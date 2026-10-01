import { useTranslation } from 'react-i18next';
import type { FocusSessionDto } from '../api/focusApi';

export function ParkedThoughts({ thoughts }: { thoughts: FocusSessionDto['parkedThoughts'] }) {
  const { t } = useTranslation();
  if (!thoughts?.length) return null;
  return (
    <details className="focus-parked-thoughts">
      <summary>{t('focus.recovery.savedThoughts', { count: thoughts.length })}</summary>
      <ul>{thoughts.map(thought => <li key={thought.id}>{thought.text}</li>)}</ul>
    </details>
  );
}

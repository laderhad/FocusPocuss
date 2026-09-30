import { useTranslation } from 'react-i18next';
import { FocusArtwork } from './FocusArtwork';

export function CompletionHeading() {
  const { t } = useTranslation();

  return (
    <>
      <FocusArtwork variant="completion" />
      <h1 id="focus-completed-title">{t('focus.session.completed')}</h1>
      <p className="focus-completion-message">{t('focus.session.completedMessage')}</p>
    </>
  );
}

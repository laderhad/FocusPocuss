import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Brand } from '../shared/components/Brand';

export function Footer() {
  const { t } = useTranslation();

  return (
    <footer className="app-footer">
      <div className="footer-content">
        <div className="footer-intro">
          <Link className="footer-brand" to="/" aria-label={`FocusPocuss — ${t('navigation.home')}`}>
            <Brand />
          </Link>
          <p>{t('footer.message')}</p>
        </div>
        <nav aria-label={t('footer.navigation')}>
          <Link to="/tasks">{t('navigation.tasks')}</Link>
          <Link to="/history">{t('navigation.history')}</Link>
        </nav>
        <div className="footer-note">
          <span>{t('footer.signature')}</span>
          <small>© {new Date().getFullYear()} FocusPocuss</small>
        </div>
      </div>
    </footer>
  );
}

import { NavMenu } from './NavMenu';
import { useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Footer } from './Footer';

export function Layout({ children }) {
  const { t } = useTranslation();
  const location = useLocation();
  const isFocusMode = location.pathname.startsWith('/focus/');

  if (isFocusMode) {
    return <main className="focus-main">{children}</main>;
  }

  return (
    <>
      <a className="skip-link" href="#main-content">{t('navigation.skipToContent')}</a>
      <NavMenu />
      <main id="main-content" tabIndex={-1}>{children}</main>
      <Footer />
    </>
  );
}

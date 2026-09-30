import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from './api-authorization/AuthContext';
import { LanguageSwitcher } from './LanguageSwitcher';
import { ThemeToggle } from './ThemeToggle';
import { Brand } from '../shared/components/Brand';

function AuthLinks() {
  const { t } = useTranslation();
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async (e) => {
    e.preventDefault();
    await logout();
    navigate('/login');
  };

  if (isAuthenticated) {
    return <li><a href="#" onClick={handleLogout}>{t('navigation.logout')}</a></li>;
  }
  return (
    <>
      <li><Link to="/login">{t('navigation.login')}</Link></li>
      <li><Link className="nav-register" to="/register">{t('navigation.register')}</Link></li>
    </>
  );
}

export function NavMenu() {
  const { t } = useTranslation();

  return (
    <header className="app-header">
      <nav aria-label={t('navigation.label')}>
        <ul className="nav-brand">
          <li><Link to="/" aria-label={`FocusPocuss — ${t('navigation.home')}`}><Brand /></Link></li>
        </ul>
        <ul className="nav-pages">
          <li><NavLink to="/" end>{t('navigation.home')}</NavLink></li>
          <li><NavLink to="/tasks">{t('navigation.tasks')}</NavLink></li>
          <li><NavLink to="/history">{t('navigation.history')}</NavLink></li>
        </ul>
        <ul className="nav-controls">
          <AuthLinks />
          <li aria-hidden="true" className="nav-separator"></li>
          <li><LanguageSwitcher /></li>
          <li><ThemeToggle /></li>
        </ul>
      </nav>
    </header>
  );
}

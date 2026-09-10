import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from './api-authorization/AuthContext';
import { LanguageSwitcher } from './LanguageSwitcher';
import { ThemeToggle } from './ThemeToggle';

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
      <li><Link to="/register">{t('navigation.register')}</Link></li>
    </>
  );
}

export function NavMenu() {
  const { t } = useTranslation();

  return (
    <header>
      <nav>
        <ul>
          <li><Link to="/">FocusPocus</Link></li>
        </ul>
        <ul>
          <li><Link to="/">{t('navigation.home')}</Link></li>
          <li><Link to="/counter">{t('navigation.counter')}</Link></li>
          <li><Link to="/weather">{t('navigation.weather')}</Link></li>
          <li><Link to="/tasks">{t('navigation.tasks')}</Link></li>
        </ul>
        <ul>
          <AuthLinks />
          <li aria-hidden="true" className="nav-separator"></li>
          <li><LanguageSwitcher /></li>
          <li><ThemeToggle /></li>
        </ul>
      </nav>
    </header>
  );
}

import { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from './AuthContext';

export function LoginPage() {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [invalid, setInvalid] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
      const returnUrl = location.state?.returnUrl || '/';
      navigate(returnUrl, { replace: true });
    } catch {
      setInvalid(true);
    }
  };

  const handleChange = (setter) => (e) => {
    setInvalid(false);
    setter(e.target.value);
  };

  return (
    <article>
      <h2>{t('auth.login.title')}</h2>
      <form onSubmit={handleSubmit}>
        <label htmlFor="email">{t('auth.login.email')}</label>
        <input type="email" id="email" autoComplete="username"
          value={email} onChange={handleChange(setEmail)}
          aria-invalid={invalid || undefined}
          aria-describedby={invalid ? 'login-error' : undefined} />
        <label htmlFor="password">{t('auth.login.password')}</label>
        <input type="password" id="password" autoComplete="current-password"
          value={password} onChange={handleChange(setPassword)}
          aria-invalid={invalid || undefined}
          aria-describedby={invalid ? 'login-error' : undefined} />
        {invalid && <small id="login-error">{t('auth.login.invalid')}</small>}
        <button type="submit">{t('auth.login.submit')}</button>
        <p style={{ marginTop: '1rem' }}>
          {t('auth.login.noAccount')} <Link to="/register">{t('auth.login.registerLink')}</Link>
        </p>
      </form>
    </article>
  );
}

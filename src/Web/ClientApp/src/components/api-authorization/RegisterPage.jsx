import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from './AuthContext';

const MIN_PASSWORD_LENGTH = 6;

function validateEmail(value) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
}

function validatePassword(value) {
  return value.length >= MIN_PASSWORD_LENGTH
    && /[a-z]/.test(value)
    && /[A-Z]/.test(value)
    && /\d/.test(value)
    && /[^A-Za-z0-9]/.test(value);
}

function isDuplicateEmailError(error) {
  if (!error || typeof error !== 'object' || !error.errors) return false;

  return Boolean(error.errors.DuplicateEmail || error.errors.DuplicateUserName);
}

export function RegisterPage() {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [emailTouched, setEmailTouched] = useState(false);
  const [passwordTouched, setPasswordTouched] = useState(false);
  const [error, setError] = useState('');
  const { register } = useAuth();
  const navigate = useNavigate();

  const emailValid = validateEmail(email);
  const passwordValid = validatePassword(password);

  const emailInvalid = emailTouched ? !emailValid : undefined;
  const passwordInvalid = passwordTouched ? !passwordValid : undefined;

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setEmailTouched(true);
    setPasswordTouched(true);
    if (!emailValid || !passwordValid) return;
    try {
      await register(email, password);
      navigate('/login');
    } catch (registrationError) {
      setError(t(
        isDuplicateEmailError(registrationError)
          ? 'auth.register.emailInUse'
          : 'auth.register.failed',
      ));
    }
  };

  return (
    <article>
      <h2>{t('auth.register.title')}</h2>
      {error && <p className="error">{error}</p>}
      <form onSubmit={handleSubmit}>
        <label htmlFor="email">{t('auth.register.email')}</label>
        <input type="email" id="email" autoComplete="username"
          value={email}
          onChange={e => setEmail(e.target.value)}
          onBlur={() => setEmailTouched(true)}
          aria-invalid={emailInvalid}
          aria-describedby="email-helper" />
        <small id="email-helper">
          {emailTouched && !emailValid ? t('auth.register.invalidEmail') : ''}
        </small>
        <label htmlFor="password">{t('auth.register.password')}</label>
        <input type="password" id="password" autoComplete="new-password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          onBlur={() => setPasswordTouched(true)}
          aria-invalid={passwordInvalid}
          aria-describedby="password-helper" />
        <small id="password-helper">
          {passwordTouched && !passwordValid
            ? t('auth.register.passwordRequirements', { count: MIN_PASSWORD_LENGTH })
            : ''}
        </small>
        <button type="submit">{t('auth.register.submit')}</button>
        <p style={{ marginTop: '1rem' }}>
          {t('auth.register.hasAccount')} <Link to="/login">{t('auth.register.loginLink')}</Link>
        </p>
      </form>
    </article>
  );
}

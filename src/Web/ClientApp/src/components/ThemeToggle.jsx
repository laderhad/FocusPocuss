import { Sun, Moon, Laptop } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { useTheme } from './ThemeContext';

const icons = {
  auto:  <Laptop size={22} strokeWidth={2} />,
  light: <Sun    size={22} strokeWidth={2} />,
  dark:  <Moon   size={22} strokeWidth={2} />,
};

const next = { auto: 'light', light: 'dark', dark: 'auto' };

export function ThemeToggle() {
  const { t } = useTranslation();
  const { theme, setTheme } = useTheme();

  return (
    <button
      className="theme-toggle-btn"
      onClick={() => setTheme(next[theme])}
      aria-label={t(`theme.${theme}`)}
      title={t(`theme.${theme}`)}
    >
      {icons[theme]}
    </button>
  );
}

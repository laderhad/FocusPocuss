import type { ChangeEvent } from 'react';
import { useTranslation } from 'react-i18next';

export function LanguageSwitcher() {
  const { i18n, t } = useTranslation();
  const language = i18n.resolvedLanguage?.startsWith('tr') ? 'tr' : 'en';

  const handleChange = (event: ChangeEvent<HTMLSelectElement>) => {
    void i18n.changeLanguage(event.target.value);
  };

  return (
    <select
      className="language-select"
      value={language}
      onChange={handleChange}
      aria-label={t('language.label')}
      title={t('language.label')}
    >
      <option value="en" lang="en">English</option>
      <option value="tr" lang="tr">Türkçe</option>
    </select>
  );
}

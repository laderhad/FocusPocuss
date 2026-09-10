import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import { en } from './locales/en';
import { tr } from './locales/tr';

const LANGUAGE_STORAGE_KEY = 'focusPocus.language';
const supportedLanguages = ['en', 'tr'] as const;

type SupportedLanguage = (typeof supportedLanguages)[number];

function isSupportedLanguage(language: string | null): language is SupportedLanguage {
  return supportedLanguages.includes(language as SupportedLanguage);
}

function getInitialLanguage(): SupportedLanguage {
  const storedLanguage = localStorage.getItem(LANGUAGE_STORAGE_KEY);
  if (isSupportedLanguage(storedLanguage)) return storedLanguage;

  return navigator.language.toLowerCase().startsWith('tr') ? 'tr' : 'en';
}

const initialLanguage = getInitialLanguage();

document.documentElement.lang = initialLanguage;

void i18n
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      tr: { translation: tr },
    },
    lng: initialLanguage,
    fallbackLng: 'en',
    supportedLngs: supportedLanguages,
    interpolation: {
      escapeValue: false,
    },
  });

i18n.on('languageChanged', language => {
  const supportedLanguage: SupportedLanguage = language.startsWith('tr') ? 'tr' : 'en';
  localStorage.setItem(LANGUAGE_STORAGE_KEY, supportedLanguage);
  document.documentElement.lang = supportedLanguage;
});

export default i18n;

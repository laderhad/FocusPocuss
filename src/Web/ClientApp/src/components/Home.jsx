import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export function Home() {
  const { t } = useTranslation();

  return (
    <section className="home-intro" aria-labelledby="home-title">
      <h1 id="home-title">{t('home.title')}</h1>
      <p>{t('home.description')}</p>
      <Link to="/tasks" role="button">{t('home.action')}</Link>
    </section>
  );
}

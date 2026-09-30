import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ArrowDown, ArrowRight } from 'lucide-react';
import contour from '../shared/assets/launchpad-contour.svg?raw';

export function Home() {
  const { t } = useTranslation();

  return (
    <>
      <section className="home-intro" aria-labelledby="home-title">
        <div className="home-contour" aria-hidden="true" dangerouslySetInnerHTML={{ __html: contour }} />
        <div className="home-content">
          <h1 id="home-title">{t('home.title')}</h1>
          <p>{t('home.description')}</p>
          <div className="home-actions">
            <Link to="/tasks" role="button">
              {t('home.action')}
              <ArrowRight size={18} aria-hidden="true" />
            </Link>
            <a className="home-explore" href="#how-it-works">
              {t('home.howItWorks')}
              <ArrowDown size={16} aria-hidden="true" />
            </a>
          </div>
        </div>
      </section>
      <section className="home-steps" id="how-it-works" aria-labelledby="home-steps-title" tabIndex={-1}>
        <header>
          <p className="home-section-label">{t('home.howItWorks')}</p>
          <h2 id="home-steps-title">{t('home.stepsTitle')}</h2>
        </header>
        <ol>
          {['capture', 'focus', 'return'].map((step, index) => (
            <li key={step}>
              <span className="home-step-number" aria-hidden="true">0{index + 1}</span>
              <h3>{t(`home.steps.${step}.title`)}</h3>
              <p>{t(`home.steps.${step}.description`)}</p>
            </li>
          ))}
        </ol>
      </section>
    </>
  );
}

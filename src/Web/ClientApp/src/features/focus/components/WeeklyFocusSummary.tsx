import { useTranslation } from 'react-i18next';
import type { FocusSessionHistoryItemDto } from '../api/focusApi';
import { getWeeklySummary } from '../model/weeklySummary';

export function WeeklyFocusSummary({ sessions }: { sessions: FocusSessionHistoryItemDto[] }) {
  const { t, i18n } = useTranslation();
  const { days, started, completed } = getWeeklySummary(sessions);
  const formatter = new Intl.DateTimeFormat(i18n.resolvedLanguage, { weekday: 'short' });
  const dateFormatter = new Intl.DateTimeFormat(i18n.resolvedLanguage, { dateStyle: 'long' });
  const numberFormatter = new Intl.NumberFormat(i18n.resolvedLanguage);
  const max = Math.max(1, ...days.map(day => day.starts));
  const points = days.map((day, index) => ({ x: 20 + index * 46, y: 54 - day.starts / max * 40 }));

  return (
    <section className="weekly-focus-summary" aria-labelledby="weekly-focus-title">
      <div>
        <h2 id="weekly-focus-title">{t('focus.history.thisWeek')}</h2>
        <dl className="weekly-focus-totals">
          <div><dt>{t('focus.history.starts')}</dt><dd>{numberFormatter.format(started)}</dd></div>
          <div><dt>{t('focus.history.completions')}</dt><dd>{numberFormatter.format(completed)}</dd></div>
        </dl>
        <p className="weekly-focus-note">{t('focus.history.weekNote')}</p>
      </div>
      <div className="weekly-focus-rhythm">
        <p>{t('focus.history.dailyStarts')}</p>
        <svg viewBox="0 0 316 64" aria-hidden="true" focusable="false">
          <line x1="20" x2="296" y1="54" y2="54" className="rhythm-baseline" />
          <polyline points={points.map(point => `${point.x},${point.y}`).join(' ')} fill="none" />
          {points.map((point, index) => <circle key={index} cx={point.x} cy={point.y} r="3" />)}
        </svg>
        <ol>
          {days.map(day => (
            <li key={day.date.getTime()} aria-label={`${dateFormatter.format(day.date)}: ${numberFormatter.format(day.starts)}`}>
              <span>{formatter.format(day.date)}</span>
              <strong>{numberFormatter.format(day.starts)}</strong>
            </li>
          ))}
        </ol>
      </div>
    </section>
  );
}

import type { FocusSessionHistoryItemDto } from '../api/focusApi';

type SessionDates = Pick<FocusSessionHistoryItemDto, 'startedAtUtc' | 'completedAtUtc'>;

export function getWeeklySummary(sessions: readonly SessionDates[], now = new Date()) {
  const monday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  monday.setDate(monday.getDate() - (monday.getDay() + 6) % 7);
  const days = Array.from({ length: 7 }, (_, index) => {
    const date = new Date(monday);
    date.setDate(date.getDate() + index);
    return { date, starts: 0 };
  });
  const end = new Date(monday);
  end.setDate(end.getDate() + 7);
  let completed = 0;

  for (const session of sessions) {
    if (session.startedAtUtc) {
      const started = session.startedAtUtc;
      const day = days.find(item => item.date.getFullYear() === started.getFullYear()
        && item.date.getMonth() === started.getMonth()
        && item.date.getDate() === started.getDate());
      if (day) day.starts++;
    }
    // Count completion events this week, including sessions started earlier.
    if (session.completedAtUtc && session.completedAtUtc >= monday && session.completedAtUtc < end) {
      completed++;
    }
  }

  return { days, started: days.reduce((total, day) => total + day.starts, 0), completed };
}

import assert from 'node:assert/strict';
import test from 'node:test';
import { getWeeklySummary } from './weeklySummary.ts';

test('uses a Monday-based local calendar week and excludes neighboring weeks', () => {
  const summary = getWeeklySummary([
    { startedAtUtc: new Date(2026, 8, 27, 23, 59) },
    { startedAtUtc: new Date(2026, 8, 28) },
    { startedAtUtc: new Date(2026, 9, 4, 23, 59) },
    { startedAtUtc: new Date(2026, 9, 5) },
  ], new Date(2026, 9, 4, 12));
  assert.equal(summary.started, 2);
  assert.deepEqual(summary.days.map(day => day.starts), [1, 0, 0, 0, 0, 0, 1]);
});

test('counts actual completion dates independently of the start week', () => {
  const summary = getWeeklySummary([
    { startedAtUtc: new Date(2026, 8, 27), completedAtUtc: new Date(2026, 8, 28) },
    { startedAtUtc: new Date(2026, 8, 28) },
    { startedAtUtc: new Date(2026, 8, 29), completedAtUtc: new Date(2026, 9, 5) },
  ], new Date(2026, 8, 30));
  assert.equal(summary.started, 2);
  assert.equal(summary.completed, 1);
});

test('handles empty history and year boundaries without modifying source dates', () => {
  const now = new Date(2027, 0, 1, 12);
  const original = now.getTime();
  const summary = getWeeklySummary([], now);
  assert.equal(summary.days[0].date.getTime(), new Date(2026, 11, 28).getTime());
  assert.equal(summary.days[6].date.getTime(), new Date(2027, 0, 3).getTime());
  assert.equal(summary.started, 0);
  assert.equal(summary.completed, 0);
  assert.equal(now.getTime(), original);
});

test('keeps calendar days aligned across daylight-saving transitions', () => {
  const summary = getWeeklySummary([
    { startedAtUtc: new Date(2026, 2, 29, 23, 59) },
  ], new Date(2026, 2, 29, 12));
  assert.deepEqual(summary.days.map(day => day.date.getDate()), [23, 24, 25, 26, 27, 28, 29]);
  assert.deepEqual(summary.days.map(day => day.starts), [0, 0, 0, 0, 0, 0, 1]);
});

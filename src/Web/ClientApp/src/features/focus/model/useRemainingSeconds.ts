import { useEffect, useState } from 'react';

function calculateRemainingSeconds(
  startedAtUtc: Date | undefined,
  durationMinutes: number | undefined,
) {
  if (!startedAtUtc || !durationMinutes) {
    return 0;
  }

  const endsAt = startedAtUtc.getTime() + durationMinutes * 60_000;
  return Math.max(0, Math.ceil((endsAt - Date.now()) / 1000));
}

export function useRemainingSeconds(
  startedAtUtc: Date | undefined,
  durationMinutes: number | undefined,
  isActive: boolean,
) {
  const [remainingSeconds, setRemainingSeconds] = useState(() =>
    calculateRemainingSeconds(startedAtUtc, durationMinutes),
  );

  useEffect(() => {
    const initialRemainingSeconds = calculateRemainingSeconds(startedAtUtc, durationMinutes);
    setRemainingSeconds(initialRemainingSeconds);

    if (!isActive || initialRemainingSeconds === 0) {
      return;
    }

    const intervalId = window.setInterval(() => {
      const nextRemainingSeconds = calculateRemainingSeconds(startedAtUtc, durationMinutes);
      setRemainingSeconds(nextRemainingSeconds);

      if (nextRemainingSeconds === 0) {
        window.clearInterval(intervalId);
      }
    }, 1_000);

    return () => window.clearInterval(intervalId);
  }, [durationMinutes, isActive, startedAtUtc]);

  return remainingSeconds;
}

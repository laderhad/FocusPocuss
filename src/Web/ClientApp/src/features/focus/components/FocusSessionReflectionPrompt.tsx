import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FocusSessionReflection } from '../api/focusApi';
import { useRecordFocusSessionReflection } from '../api/focusQueries';
import { CompletionHeading } from './CompletionHeading';

const reflectionOptions = [
  {
    value: FocusSessionReflection.FocusedWell,
    translationKey: 'focus.reflection.options.focusedWell',
  },
  {
    value: FocusSessionReflection.SomeDifficulty,
    translationKey: 'focus.reflection.options.someDifficulty',
  },
  {
    value: FocusSessionReflection.SignificantDifficulty,
    translationKey: 'focus.reflection.options.significantDifficulty',
  },
] as const;

interface FocusSessionReflectionPromptProps {
  sessionId: number;
  onSkip: () => void;
}

export function FocusSessionReflectionPrompt({
  sessionId,
  onSkip,
}: FocusSessionReflectionPromptProps) {
  const { t } = useTranslation();
  const recordReflection = useRecordFocusSessionReflection(sessionId);
  const [selectedReflection, setSelectedReflection] =
    useState<FocusSessionReflection>();

  const selectReflection = (reflection: FocusSessionReflection) => {
    setSelectedReflection(reflection);
    recordReflection.mutate(reflection);
  };

  return (
    <section className="focus-reflection" aria-labelledby="focus-reflection-title">
      <CompletionHeading />
      <h2 id="focus-reflection-title">{t('focus.reflection.title')}</h2>

      <div className="focus-reflection-options">
        {reflectionOptions.map(option => (
          <button
            key={option.value}
            type="button"
            className="secondary outline"
            disabled={recordReflection.isPending}
            onClick={() => selectReflection(option.value)}
          >
            {recordReflection.isPending && selectedReflection === option.value
              ? t('focus.reflection.saving')
              : t(option.translationKey)}
          </button>
        ))}
      </div>

      <button
        type="button"
        className="secondary focus-reflection-skip"
        disabled={recordReflection.isPending}
        onClick={onSkip}
      >
        {t('focus.reflection.skip')}
      </button>

      {recordReflection.isError && (
        <p className="focus-session-error" role="alert">
          {t('focus.reflection.error')}
        </p>
      )}
    </section>
  );
}

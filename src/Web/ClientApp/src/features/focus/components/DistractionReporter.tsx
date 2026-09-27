import { CircleAlert } from 'lucide-react';
import { useState, type FormEvent } from 'react';
import { useTranslation } from 'react-i18next';
import { DistractionReason } from '../api/focusApi';
import { useReportDistraction } from '../api/focusQueries';

const reasonOptions = [
  {
    value: DistractionReason.TaskTooDifficult,
    translationKey: 'focus.distraction.reasons.taskTooDifficult',
  },
  {
    value: DistractionReason.UnclearNextAction,
    translationKey: 'focus.distraction.reasons.unclearNextAction',
  },
  {
    value: DistractionReason.PhoneOrSocialMedia,
    translationKey: 'focus.distraction.reasons.phoneOrSocialMedia',
  },
  {
    value: DistractionReason.AnotherThought,
    translationKey: 'focus.distraction.reasons.anotherThought',
  },
  {
    value: DistractionReason.Tired,
    translationKey: 'focus.distraction.reasons.tired',
  },
  {
    value: DistractionReason.Other,
    translationKey: 'focus.distraction.reasons.other',
  },
] as const;

interface DistractionReporterProps {
  sessionId: number;
}

export function DistractionReporter({ sessionId }: DistractionReporterProps) {
  const { t } = useTranslation();
  const reportDistraction = useReportDistraction(sessionId);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedReason, setSelectedReason] = useState<DistractionReason>();
  const [isAcknowledged, setIsAcknowledged] = useState(false);

  const open = () => {
    reportDistraction.reset();
    setIsAcknowledged(false);
    setIsOpen(true);
  };

  const cancel = () => {
    reportDistraction.reset();
    setSelectedReason(undefined);
    setIsOpen(false);
  };

  const submit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (selectedReason === undefined) {
      return;
    }

    reportDistraction.mutate(selectedReason, {
      onSuccess: () => {
        setSelectedReason(undefined);
        setIsOpen(false);
        setIsAcknowledged(true);
      },
    });
  };

  return (
    <section className="focus-distraction" aria-label={t('focus.distraction.title')}>
      {!isOpen && (
        <button
          type="button"
          className="secondary outline focus-distraction-trigger"
          onClick={open}
        >
          <CircleAlert size={18} aria-hidden="true" />
          {t('focus.distraction.action')}
        </button>
      )}

      {isOpen && (
        <form className="focus-distraction-form" onSubmit={submit}>
          <fieldset disabled={reportDistraction.isPending}>
            <legend>{t('focus.distraction.title')}</legend>
            {reasonOptions.map(option => {
              const inputId = `distraction-reason-${option.value}`;

              return (
                <label key={option.value} htmlFor={inputId}>
                  <input
                    id={inputId}
                    type="radio"
                    name="distraction-reason"
                    value={option.value}
                    checked={selectedReason === option.value}
                    onChange={() => setSelectedReason(option.value)}
                    required
                  />
                  {t(option.translationKey)}
                </label>
              );
            })}
          </fieldset>

          <div className="focus-distraction-actions">
            <button
              type="button"
              className="secondary outline"
              disabled={reportDistraction.isPending}
              onClick={cancel}
            >
              {t('focus.distraction.cancel')}
            </button>
            <button
              type="submit"
              disabled={selectedReason === undefined || reportDistraction.isPending}
            >
              {reportDistraction.isPending
                ? t('focus.distraction.saving')
                : t('focus.distraction.submit')}
            </button>
          </div>

          {reportDistraction.isError && (
            <p className="focus-session-error" role="alert">
              {t('focus.distraction.error')}
            </p>
          )}
        </form>
      )}

      {isAcknowledged && (
        <p className="focus-distraction-success" role="status">
          {t('focus.distraction.success')}
        </p>
      )}
    </section>
  );
}

import { ArrowRight, Check } from 'lucide-react';
import { useEffect, useRef, useState, type FormEvent } from 'react';
import { useTranslation } from 'react-i18next';
import { DistractionReason, InterventionStrategy } from '../api/focusApi';
import { useReportDistraction } from '../api/focusQueries';
import { FocusArtwork } from './FocusArtwork';

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

const interventionMessageKeys: Record<InterventionStrategy, string> = {
  [InterventionStrategy.TaskDecomposition]:
    'focus.distraction.interventions.taskDecomposition',
  [InterventionStrategy.ClarifyNextAction]:
    'focus.distraction.interventions.clarifyNextAction',
  [InterventionStrategy.RemoveFriction]:
    'focus.distraction.interventions.removeFriction',
  [InterventionStrategy.DistractionRecovery]:
    'focus.distraction.interventions.distractionRecovery',
  [InterventionStrategy.BreakRecommendation]:
    'focus.distraction.interventions.breakRecommendation',
};

interface DistractionReporterProps {
  sessionId: number;
}

export function DistractionReporter({ sessionId }: DistractionReporterProps) {
  const { t } = useTranslation();
  const reportDistraction = useReportDistraction(sessionId);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedReason, setSelectedReason] = useState<DistractionReason>();
  const [isAcknowledged, setIsAcknowledged] = useState(false);
  const [selectedStrategy, setSelectedStrategy] = useState<InterventionStrategy>();
  const dialogRef = useRef<HTMLDialogElement>(null);
  const guidanceRef = useRef<HTMLParagraphElement>(null);
  const isRecovering = isOpen || isAcknowledged;

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (isRecovering && !dialog.open) dialog.showModal();
    if (!isRecovering && dialog.open) dialog.close();
  }, [isRecovering]);

  useEffect(() => {
    if (isAcknowledged) guidanceRef.current?.focus();
  }, [isAcknowledged]);

  const open = () => {
    reportDistraction.reset();
    setIsAcknowledged(false);
    setSelectedStrategy(undefined);
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
      onSuccess: report => {
        setSelectedReason(undefined);
        setSelectedStrategy(report.strategy);
        setIsOpen(false);
        setIsAcknowledged(true);
      },
    });
  };

  const returnToFocus = () => {
    reportDistraction.reset();
    setSelectedStrategy(undefined);
    setIsAcknowledged(false);
  };

  return (
    <section className="focus-distraction" aria-label={t('focus.distraction.title')}>
      <button
        type="button"
        className="secondary outline focus-distraction-trigger"
        onClick={open}
      >
        {t('focus.distraction.action')}
      </button>

      <dialog
        ref={dialogRef}
        className="focus-recovery-dialog"
        aria-labelledby="focus-recovery-title"
        onCancel={event => {
          event.preventDefault();
          if (!reportDistraction.isPending) {
            if (isAcknowledged) returnToFocus();
            else cancel();
          }
        }}
      >
        <div className="focus-recovery-content">
          <FocusArtwork variant="recovery" />
          <h1 id="focus-recovery-title">{t('focus.distraction.heading')}</h1>

          {isOpen && (
            <form className="focus-distraction-form" onSubmit={submit}>
              <fieldset disabled={reportDistraction.isPending}>
                <legend>{t('focus.distraction.title')}</legend>
                <div className="focus-reason-options">
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
                        <span>
                          <Check className="focus-reason-check" size={16} aria-hidden="true" />
                          {t(option.translationKey)}
                        </span>
                      </label>
                    );
                  })}
                </div>
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
                  aria-busy={reportDistraction.isPending}
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
            <div className="focus-distraction-success" role="status">
              <p className="focus-distraction-success-title">
                {t('focus.distraction.recoveryTitle')}
              </p>
              <p className="focus-distraction-message" tabIndex={-1} ref={guidanceRef}>
                {t(
                  selectedStrategy === undefined
                    ? 'focus.distraction.success'
                    : interventionMessageKeys[selectedStrategy],
                )}
              </p>
              <button
                type="button"
                className="focus-distraction-return"
                onClick={returnToFocus}
              >
                {t('focus.distraction.returnToFocus')}
                <ArrowRight size={18} aria-hidden="true" />
              </button>
            </div>
          )}
        </div>
      </dialog>
    </section>
  );
}

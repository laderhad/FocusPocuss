import { Check } from 'lucide-react';
import { useEffect, useRef, useState, type FormEvent } from 'react';
import { useTranslation } from 'react-i18next';
import { DistractionReason, RecoveryResolution, PrepareRecoveryRequest, ResolveRecoveryRequest, type DistractionReportDto, type RecoveryChoice } from '../api/focusApi';
import { usePrepareRecovery, useResolveRecovery, useReportDistraction } from '../api/focusQueries';
import { FocusArtwork } from './FocusArtwork';
import { RecoveryInterventionView } from './RecoveryInterventionView';

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
  recovery?: DistractionReportDto;
}

export function DistractionReporter({ sessionId, recovery }: DistractionReporterProps) {
  const { t, i18n } = useTranslation();
  const reportDistraction = useReportDistraction(sessionId);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedReason, setSelectedReason] = useState<DistractionReason>();
  const prepare = usePrepareRecovery(sessionId);
  const resolve = useResolveRecovery(sessionId);
  const busy = reportDistraction.isPending || prepare.isPending || resolve.isPending;
  const dialogRef = useRef<HTMLDialogElement>(null);
  const headingRef = useRef<HTMLHeadingElement>(null);
  const isRecovering = isOpen || !!recovery;

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (isRecovering && !dialog.open) dialog.showModal();
    if (!isRecovering && dialog.open) dialog.close();
  }, [isRecovering]);

  useEffect(() => {
    if (recovery) headingRef.current?.focus();
  }, [recovery]);

  const open = () => {
    reportDistraction.reset();
    prepare.reset();
    resolve.reset();
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

    reportDistraction.mutate({ reason: selectedReason, language: i18n.resolvedLanguage?.startsWith('tr') ? 'tr' : 'en' }, {
      onSuccess: () => {
        setSelectedReason(undefined);
        setIsOpen(false);
      },
    });
  };

  const onResolve = (resolution: RecoveryResolution, thought?: string) => {
    if (recovery?.id === undefined) return;
    resolve.mutate({ id: recovery.id, request: new ResolveRecoveryRequest({ resolution, thought }) },
      { onSuccess: () => setIsOpen(false) });
  };
  const onPrepare = (choice?: RecoveryChoice, clarification?: string) => {
    if (recovery?.id === undefined) return;
    prepare.mutate({ id: recovery.id, request: new PrepareRecoveryRequest({ choice, clarification }) });
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
          if (!busy) {
            if (recovery) onResolve(RecoveryResolution.Dismiss);
            else cancel();
          }
        }}
      >
        <div className="focus-recovery-content">
          <FocusArtwork variant="recovery" />
          <h1 id="focus-recovery-title" ref={headingRef} tabIndex={-1}>{t('focus.distraction.heading')}</h1>

          {isOpen && !recovery && (
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

          {recovery && (
            <RecoveryInterventionView key={`${recovery.id}-${recovery.intervention?.requirement}`} report={recovery}
              busy={busy} onPrepare={onPrepare} onResolve={onResolve} />
          )}
          {(prepare.isError || resolve.isError) && <p role="alert">{t('focus.recovery.error')}</p>}

        </div>
      </dialog>
    </section>
  );
}

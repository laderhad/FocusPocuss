import { useState, type FormEvent } from 'react';
import { useTranslation } from 'react-i18next';
import {
  RecoveryChoice, RecoveryInterventionType, RecoveryRequirement, RecoveryResolution,
  type DistractionReportDto,
} from '../api/focusApi';

interface Props {
  report: DistractionReportDto;
  busy: boolean;
  onPrepare: (choice?: RecoveryChoice, clarification?: string) => void;
  onResolve: (resolution: RecoveryResolution, thought?: string) => void;
}

export function RecoveryInterventionView({ report, busy, onPrepare, onResolve }: Props) {
  const { t } = useTranslation();
  const [text, setText] = useState('');
  const intervention = report.intervention;
  if (!intervention) return null;
  const needsThought = intervention.requirement === RecoveryRequirement.ThoughtCapture;
  const needsClarification = intervention.requirement === RecoveryRequirement.NeedsClarification;
  const unavailable = intervention.requirement === RecoveryRequirement.Unavailable
    || intervention.requirement === RecoveryRequirement.ActionTransformation;
  const needsChoice = intervention.requirement === RecoveryRequirement.UserChoice;
  const needsInput = needsThought || needsClarification;
  const submit = (event: FormEvent) => {
    event.preventDefault();
    if (needsClarification) onPrepare(undefined, text.trim());
    else onResolve(RecoveryResolution.ReturnToFocus, needsThought ? text.trim() : undefined);
  };
  const instruction = needsClarification ? 'clarification'
    : unavailable ? 'unavailable'
    : report.choice === RecoveryChoice.TakeShortReset ? 'reset'
    : intervention.type === RecoveryInterventionType.EnvironmentalReset ? 'phone'
    : needsThought ? 'thought'
    : needsChoice ? 'choice'
    : intervention.type === RecoveryInterventionType.ReconnectToCurrentAction ? 'reconnect'
    : 'changed';

  return (
    <form className="focus-distraction-success" onSubmit={submit}>
      <p className="focus-distraction-success-title">{t(`focus.recovery.titles.${intervention.type}`)}</p>
      <p className="focus-distraction-message" role="status">{t(`focus.recovery.${instruction}`)}</p>
      {intervention.returnAction && !needsChoice && (
        <div className="focus-recovery-action">
          <span>{t('focus.recovery.returnAction')}</span>
          <p>{intervention.returnAction}</p>
        </div>
      )}
      {needsInput && (
        <label htmlFor="recovery-input">
          {t(needsThought ? 'focus.recovery.thoughtLabel' : 'focus.recovery.clarificationLabel')}
          <textarea id="recovery-input" value={text} onChange={event => setText(event.target.value)}
            maxLength={needsThought ? 1000 : 500} rows={3} required disabled={busy} />
        </label>
      )}
      {needsChoice ? (
        <div className="focus-recovery-choices">
          {intervention.choices?.map(choice => (
            <button key={choice} type="button" disabled={busy} className="secondary outline"
              onClick={() => choice === RecoveryChoice.EndSession
                ? onResolve(RecoveryResolution.EndSession) : onPrepare(choice)}>
              {t(`focus.recovery.choices.${choice}`)}
            </button>
          ))}
        </div>
      ) : !unavailable && (
        <button className="focus-distraction-return" type="submit" aria-busy={busy}
          disabled={busy || (needsInput && !text.trim())}>
          {t(needsThought ? 'focus.recovery.saveReturn' : needsClarification
            ? 'focus.recovery.clarify' : 'focus.distraction.returnToFocus')}
        </button>
      )}
      {(unavailable || needsClarification) && (
        <button type="button" className="secondary outline" disabled={busy}
          onClick={() => onResolve(RecoveryResolution.KeepCurrentAction)}>
          {t('focus.recovery.keepAction')}
        </button>
      )}
      <button type="button" className="secondary outline" disabled={busy}
        onClick={() => onResolve(RecoveryResolution.Dismiss)}>{t('focus.recovery.dismiss')}</button>
    </form>
  );
}

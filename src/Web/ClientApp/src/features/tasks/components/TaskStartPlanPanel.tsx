import { useEffect } from 'react';
import { Hourglass, RefreshCw } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { StartFocusSessionButton } from '../../focus';
import { useCreateTaskStartPlan } from '../api/taskQueries';
import type { TaskStartPlanDto } from '../api/tasksApi';

interface TaskStartPlanPanelProps {
  taskId: number;
  existingPlan?: TaskStartPlanDto;
}

export function TaskStartPlanPanel({ taskId, existingPlan }: TaskStartPlanPanelProps) {
  const { i18n, t } = useTranslation();
  const {
    data: generatedPlan,
    isError,
    isIdle,
    mutate,
  } = useCreateTaskStartPlan(taskId);
  const language = i18n.resolvedLanguage?.startsWith('tr') ? 'tr' : 'en';
  const plan = existingPlan ?? generatedPlan;

  useEffect(() => {
    if (!existingPlan && isIdle) {
      mutate(language);
    }
  }, [existingPlan, isIdle, language, mutate]);

  if (plan) {
    return (
      <section className="task-start-plan" aria-labelledby="task-start-plan-title">
        <div className="task-start-plan-meta">
          <p className="task-start-plan-label">{t('tasks.startPlan.title')}</p>
          {plan.suggestedDurationMinutes !== undefined && (
            <p className="task-start-plan-duration">
              <Hourglass size={14} aria-hidden="true" />
              {t('tasks.startPlan.duration', { count: plan.suggestedDurationMinutes })}
            </p>
          )}
        </div>
        <h2 id="task-start-plan-title" className="task-start-plan-action">
          {plan.nextAction}
        </h2>
        <p className="task-start-plan-message">{plan.message}</p>
        <div className="task-start-plan-footer">
          {plan.id !== undefined && (
            <StartFocusSessionButton taskId={taskId} taskStartPlanId={plan.id} />
          )}
        </div>
      </section>
    );
  }

  if (isError) {
    return (
      <section className="task-start-plan task-start-plan-state" role="alert">
        <p>{t('tasks.startPlan.error')}</p>
        <button type="button" className="secondary outline" onClick={() => mutate(language)}>
          <RefreshCw size={18} aria-hidden="true" />
          {t('tasks.startPlan.retry')}
        </button>
      </section>
    );
  }

  return (
    <section className="task-start-plan task-start-plan-state" aria-live="polite">
      <p>{t('tasks.startPlan.generating')}</p>
    </section>
  );
}

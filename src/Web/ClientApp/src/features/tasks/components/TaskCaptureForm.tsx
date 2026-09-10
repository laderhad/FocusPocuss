import { useState, type FormEvent } from 'react';
import { Plus } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { useCreateTask } from '../api/taskQueries';

const maxOriginalInputLength = 1000;

type ValidationError = 'required' | 'tooLong';

export function TaskCaptureForm() {
  const { t } = useTranslation();
  const [originalInput, setOriginalInput] = useState('');
  const [validationError, setValidationError] = useState<ValidationError | null>(null);
  const createTask = useCreateTask();

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (originalInput.trim().length === 0) {
      setValidationError('required');
      return;
    }

    if (originalInput.length > maxOriginalInputLength) {
      setValidationError('tooLong');
      return;
    }

    setValidationError(null);
    createTask.mutate(originalInput, {
      onSuccess: () => setOriginalInput(''),
    });
  };

  const handleInputChange = (value: string) => {
    setOriginalInput(value);
    setValidationError(null);

    if (createTask.isError) {
      createTask.reset();
    }
  };

  const errorMessage = validationError
    ? t(`tasks.capture.validation.${validationError}`, { count: maxOriginalInputLength })
    : createTask.isError
      ? t('tasks.capture.saveError')
      : null;

  return (
    <form className="task-capture-form" onSubmit={handleSubmit} noValidate>
      <label htmlFor="task-original-input">{t('tasks.capture.label')}</label>
      <textarea
        id="task-original-input"
        name="originalInput"
        value={originalInput}
        onChange={(event) => handleInputChange(event.target.value)}
        placeholder={t('tasks.capture.placeholder')}
        maxLength={maxOriginalInputLength}
        rows={4}
        aria-invalid={errorMessage ? true : undefined}
        aria-describedby={errorMessage ? 'task-capture-error' : undefined}
        disabled={createTask.isPending}
      />
      <div className="task-capture-footer">
        <div id="task-capture-error" className="task-capture-error" aria-live="polite">
          {errorMessage}
        </div>
        <button type="submit" disabled={createTask.isPending}>
          <Plus size={18} aria-hidden="true" />
          {createTask.isPending
            ? t('tasks.capture.submitting')
            : t('tasks.capture.submit')}
        </button>
      </div>
    </form>
  );
}

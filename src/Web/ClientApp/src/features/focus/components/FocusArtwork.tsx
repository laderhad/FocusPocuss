import focusField from '../../../shared/assets/focus-field.svg?raw';
import recoveryFragments from '../../../shared/assets/recovery-fragments.svg?raw';
import completionField from '../../../shared/assets/completion-field.svg?raw';

const artwork = { focus: focusField, recovery: recoveryFragments, completion: completionField };

export function FocusArtwork({ variant }: { variant: keyof typeof artwork }) {
  // Only trusted local SVGs; inline markup inherits light/dark semantic colors.
  return (
    <div
      className={`focus-artwork focus-artwork-${variant}`}
      aria-hidden="true"
      dangerouslySetInnerHTML={{ __html: artwork[variant] }}
    />
  );
}

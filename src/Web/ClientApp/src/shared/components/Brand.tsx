import brand from '../assets/focuspocuss-brand.svg?raw';

// Static local Stitch artwork. The enclosing link supplies the accessible name.
export function Brand() {
  return <span className="brand" aria-hidden="true" dangerouslySetInnerHTML={{ __html: brand }} />;
}

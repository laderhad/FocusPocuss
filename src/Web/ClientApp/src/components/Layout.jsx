import { NavMenu } from './NavMenu';
import { useLocation } from 'react-router-dom';

export function Layout({ children }) {
  const location = useLocation();
  const isFocusMode = location.pathname.startsWith('/focus/');

  if (isFocusMode) {
    return <main className="focus-main">{children}</main>;
  }

  return (
    <>
      <NavMenu />
      <main>{children}</main>
    </>
  );
}

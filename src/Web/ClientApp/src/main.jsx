import './styles.scss';
import './app/i18n';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import { AppProviders } from './app/AppProviders';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const root = createRoot(document.getElementById('root'));

root.render(
  <AppProviders>
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>
  </AppProviders>
);

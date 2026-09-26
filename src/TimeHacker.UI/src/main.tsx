import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import '@fontsource-variable/inter';
import 'theme/fonts.css';
import { installCssVariables } from 'theme/cssVariables';
import { App } from './App';
import './index.css';

installCssVariables();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>
);

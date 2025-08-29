import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';

import App from './App';
import { ThemeProvider } from './contexts/ThemeContext';

import './index.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Router>
      <ThemeProvider>
        <AuthProvider>
            <App />
        </AuthProvider>
      </ThemeProvider>
    </Router>
  </StrictMode>
)

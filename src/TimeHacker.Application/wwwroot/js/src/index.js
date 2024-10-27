import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { ThemeProvider } from './contexts/ThemeContext';
import { UserDataProvider } from './contexts/UserDataContext'

const rootElement = document.getElementById('calculator_react');
const root = ReactDOM.createRoot(rootElement);

root.render(
  <React.StrictMode>
    <BrowserRouter>
      {/* !!!TODO */}
      {/* <UserDataProvider> */}
        <ThemeProvider>
          <App />
        </ThemeProvider>
      {/* </UserDataProvider> */}
    </BrowserRouter>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

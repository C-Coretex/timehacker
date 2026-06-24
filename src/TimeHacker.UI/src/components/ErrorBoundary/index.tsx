import { Component } from 'react';
import { Button, Result } from 'antd';
import { useTranslation } from 'react-i18next';
import type { ErrorBoundaryProps, ErrorBoundaryState } from './types';

const ErrorFallback = () => {
  const { t } = useTranslation();
  return (
    <Result
      status="error"
      title={t('errors.title')}
      subTitle={t('errors.subtitle')}
      extra={
        <Button type="primary" onClick={() => window.location.reload()}>
          {t('errors.reload')}
        </Button>
      }
    />
  );
};

/** Catches render-time errors anywhere below it and shows a recoverable fallback. */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(): ErrorBoundaryState {
    return { hasError: true };
  }

  componentDidCatch(error: Error) {
    if (import.meta.env.DEV) {
      console.error('Unhandled render error:', error);
    }
  }

  render() {
    return this.state.hasError ? <ErrorFallback /> : this.props.children;
  }
}

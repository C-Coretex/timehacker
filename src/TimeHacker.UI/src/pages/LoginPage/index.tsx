import { useState, useMemo } from 'react';
import type { FC } from 'react';
import { Alert, Form, Tabs, Typography, message } from 'antd';
import { useNavigate, useLocation, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import api from '../../api/api';
import { useAuth } from '../../contexts/AuthContext';
import LoginForm from './components/LoginForm';
import RegisterForm from './components/RegisterForm';
import type { LoginFormData, RegisterFormData } from './types';

const LoginPage: FC = () => {
  const { t } = useTranslation();
  const { fetchCurrentUser } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams] = useSearchParams();

  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [loading, setLoading] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loginForm] = Form.useForm();
  const [registerForm] = Form.useForm();

  const authMessage = useMemo(() => {
    if (searchParams.get('expired') === 'true') return t('login.sessionExpired');
    return (location.state as { message?: string })?.message ?? null;
  }, [location.state, searchParams, t]);

  const handleLogin = async (values: LoginFormData) => {
    setLoading(true);
    setError(null);
    try {
      await api.post(`/login?useCookies=true&useSessionCookies=${!rememberMe}`, values);
      await fetchCurrentUser();
      navigate('/');
    } catch (err: unknown) {
      const msg = err && typeof err === 'object' && 'response' in err
        ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
        : null;
      setError(msg ?? t('login.loginFailed'));
    } finally {
      setLoading(false);
    }
  };

  const handleDevLogin = () => handleLogin({ email: 'test@aa.bb', password: 'Qwerty123' });

  const handleRegister = async (values: RegisterFormData) => {
    setLoading(true);
    setError(null);
    try {
      await api.post('/register', { email: values.email, password: values.password });
      await api.post(`/login?useCookies=true&useSessionCookies=${!rememberMe}`, {
        email: values.email,
        password: values.password,
      });
      await fetchCurrentUser();
      message.success(t('login.accountCreated'));
      registerForm.resetFields();
      navigate('/');
    } catch (err: unknown) {
      const msg = err && typeof err === 'object' && 'response' in err
        ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
        : null;
      setError(msg ?? t('login.registrationFailed'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 480, margin: '2rem auto', padding: '0 16px' }}>
      <Typography.Title level={2} style={{ marginBottom: 0 }}>
        {t('login.welcome')}
      </Typography.Title>
      <Typography.Text type="secondary">{t('login.subtitle')}</Typography.Text>

      {authMessage && (
        <Alert type="warning" title={authMessage} showIcon style={{ marginTop: '1rem' }} />
      )}
      {error && (
        <Alert type="error" title={error} style={{ marginTop: '1rem' }} />
      )}

      <Tabs
        activeKey={activeTab}
        onChange={(k) => setActiveTab(k as 'login' | 'register')}
        style={{ marginTop: '1rem' }}
        items={[
          {
            key: 'login',
            label: t('login.loginButton'),
            children: (
              <LoginForm
                form={loginForm}
                loading={loading}
                rememberMe={rememberMe}
                onRememberMeChange={setRememberMe}
                onFinish={handleLogin}
                onDevLogin={handleDevLogin}
              />
            ),
          },
          {
            key: 'register',
            label: t('login.register'),
            children: (
              <RegisterForm
                form={registerForm}
                loading={loading}
                onFinish={handleRegister}
              />
            ),
          },
        ]}
      />
    </div>
  );
};

export default LoginPage;

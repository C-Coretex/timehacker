import {
  Form,
  Input,
  Button,
  Typography,
  Alert,
  Tabs,
  Checkbox,
  message,
} from 'antd';
import type { TabsProps } from 'antd';
import {
  MailOutlined,
  LockOutlined,
  LoginOutlined,
  ThunderboltOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation, useSearchParams } from 'react-router-dom';
import { useState, useMemo } from 'react';
import type { FC } from 'react';
import { useTranslation } from 'react-i18next';
import api from '../api/api';
import { useAuth } from '../contexts/AuthContext';

// constants for reuse
const MIN_PASSWORD_LENGTH = 6;

const LoginPage: FC = () => {
  const { t } = useTranslation();
  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [loading, setLoading] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const { fetchCurrentUser } = useAuth();
  const [loginForm] = Form.useForm();
  const [registerForm] = Form.useForm();

  const authMessage = useMemo(() => {
    if (searchParams.get('expired') === 'true') {
      return t('login.sessionExpired');
    }
    return (location.state as { message?: string })?.message ?? null;
  }, [location.state, searchParams, t]);

  // Handle login
  const handleLogin = async (values: { email: string; password: string }) => {
    setLoading(true);
    setError(null);
    try {
      await api.post(`/login?useCookies=true&useSessionCookies=${!rememberMe}`, values);
      await fetchCurrentUser();
      navigate('/'); // redirect to calendar
    } catch (err: any) {
      setError(err.response?.data?.message || t('login.loginFailed'));
    } finally {
      setLoading(false);
    }
  };

  // Quick login for developers (shown only on Login tab)
  const handleDevLogin = () =>
    handleLogin({
      email: 'test@aa.bb',
      password: 'Qwerty123',
    });

  // Handle register (auto-login after success)
  const handleRegister = async (values: {
    email: string;
    password: string;
    confirmPassword: string;
    agree: boolean;
  }) => {
    setLoading(true);
    setError(null);
    try {
      await api.post('/register', {
        email: values.email,
        password: values.password,
      });

      // Auto-login after successful registration
      await api.post(`/login?useCookies=true&useSessionCookies=${!rememberMe}`, {
        email: values.email,
        password: values.password,
      });
      await fetchCurrentUser();
      message.success(t('login.accountCreated'));
      registerForm.resetFields();
      navigate('/');
    } catch (err: any) {
      setError(err.response?.data?.message || t('login.registrationFailed'));
    } finally {
      setLoading(false);
    }
  };

  const emailField = (
    <Form.Item
      label={t('login.email')}
      name="email"
      rules={[
        { required: true, message: t('login.emailRequired') },
        { type: 'email', message: t('login.emailInvalid') },
      ]}
      extra={t('login.emailHint')}
    >
      <Input prefix={<MailOutlined />} placeholder="example@email.com" />
    </Form.Item>
  );

  const passwordField = (name = 'password', label = t('login.password')) => (
    <Form.Item
      label={label}
      name={name}
      rules={[
        { required: true, message: t('login.passwordRequired') },
        {
          min: MIN_PASSWORD_LENGTH,
          message: t('login.passwordMinLength', { min: MIN_PASSWORD_LENGTH }),
        },
      ]}
      extra={t('login.passwordHint')}
    >
      <Input.Password prefix={<LockOutlined />} placeholder="••••••••" />
    </Form.Item>
  );

  const loginTab: NonNullable<TabsProps['items']>[number] = {
    key: 'login',
    label: t('login.loginButton'),
    children: (
      <Form
        form={loginForm}
        layout="vertical"
        onFinish={handleLogin}
        style={{ marginTop: '1rem' }}
      >
        {emailField}
        {passwordField()}
        <Form.Item>
          <Checkbox checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)}>
            {t('login.rememberMe')}
          </Checkbox>
        </Form.Item>
        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            icon={<LoginOutlined />}
            loading={loading}
            block
          >
            {t('login.loginButton')}
          </Button>
        </Form.Item>
        <Form.Item>
          <Button
            type="dashed"
            icon={<ThunderboltOutlined />}
            onClick={handleDevLogin}
            block
          >
            {t('login.devLogin')}
          </Button>
        </Form.Item>
      </Form>
    ),
  };

  const registerTab: NonNullable<TabsProps['items']>[number] = {
    key: 'register',
    label: t('login.register'),
    children: (
      <Form
        form={registerForm}
        layout="vertical"
        onFinish={handleRegister}
        style={{ marginTop: '1rem' }}
      >
        {emailField}
        {passwordField()}
        {/* Confirm password with cross-field validation */}
        <Form.Item
          label={t('login.confirmPassword')}
          name="confirmPassword"
          dependencies={['password']}
          rules={[
            { required: true, message: t('login.confirmPasswordRequired') },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue('password') === value) {
                  return Promise.resolve();
                }
                return Promise.reject(new Error(t('login.passwordsMismatch')));
              },
            }),
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder={t('login.repeatPasswordPlaceholder')}
          />
        </Form.Item>

        {/* Terms checkbox */}
        <Form.Item
          name="agree"
          valuePropName="checked"
          rules={[
            {
              validator: (_, value) =>
                value
                  ? Promise.resolve()
                  : Promise.reject(new Error(t('login.termsRequired'))),
            },
          ]}
          extra={t('login.termsHint')}
        >
          <Checkbox>{t('login.termsCheckbox')}</Checkbox>
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={loading} block>
            {t('login.createAccount')}
          </Button>
        </Form.Item>
      </Form>
    ),
  };

  return (
    <div style={{ maxWidth: 480, margin: '2rem auto', padding: '0 16px' }}>
      <Typography.Title level={2} style={{ marginBottom: 0 }}>
        {t('login.welcome')}
      </Typography.Title>
      <Typography.Text type="secondary">
        {t('login.subtitle')}
      </Typography.Text>

      {authMessage && (
        <Alert type="warning" message={authMessage} showIcon style={{ marginTop: '1rem' }} />
      )}

      {error && (
        <Alert type="error" message={error} style={{ marginTop: '1rem' }} />
      )}

      <Tabs
        activeKey={activeTab}
        onChange={(k) => setActiveTab(k as 'login' | 'register')}
        items={[loginTab, registerTab]}
        style={{ marginTop: '1rem' }}
      />
    </div>
  );
};

export default LoginPage;

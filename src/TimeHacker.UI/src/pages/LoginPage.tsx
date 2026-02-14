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
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import type { FC } from 'react';
import api from '../api/api';
import { useAuth } from '../contexts/AuthContext';

// constants for reuse
const MIN_PASSWORD_LENGTH = 6;
// Small reusable field components
const EmailField: React.FC = () => (
  <Form.Item
    label="Email"
    name="email"
    rules={[
      { required: true, message: 'Please enter your email' },
      { type: 'email', message: 'Invalid email format' },
    ]}
    extra="Use a valid email address. We will use it for notifications."
  >
    <Input prefix={<MailOutlined />} placeholder="example@email.com" />
  </Form.Item>
);

const PasswordField: React.FC<{ name?: string; label?: string }> = ({
  name = 'password',
  label = 'Password',
}) => (
  <Form.Item
    label={label}
    name={name}
    rules={[
      { required: true, message: 'Please enter your password' },
      {
        min: MIN_PASSWORD_LENGTH,
        message: `Password must be at least ${MIN_PASSWORD_LENGTH} characters`,
      },
    ]}
    extra="Use at least 6 characters. Strong passwords mix letters and numbers."
  >
    <Input.Password prefix={<LockOutlined />} placeholder="��������" />
  </Form.Item>
);

const LoginPage: FC = () => {
  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [loading, setLoading] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const { login, fetchCurrentUser } = useAuth();
  const [loginForm] = Form.useForm();
  const [registerForm] = Form.useForm();

  // Handle login
  const handleLogin = async (values: { email: string; password: string }) => {
    setLoading(true);
    setError(null);
    try {
      await api.post(`/login?useCookies=true&useSessionCookies=${!rememberMe}`, values);
      // Optionally hydrate user profile from API for breadcrumbs/menu
      await fetchCurrentUser();
      // You may also store minimal info immediately:
      login({
        name: '',
        phoneNumberForNotifications: '',
        emailForNotifications: values.email,
      });
      navigate('/'); // redirect to calendar
    } catch (err: any) {
      setError(err.response?.data?.message || 'Login failed');
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

      // after successful registration: switch to Login tab
      message.success('Account created. Please sign in.');
      setActiveTab('login');

      // Prefill email on the Login form and clear Register form
      loginForm.setFieldsValue({ email: values.email });
      registerForm.resetFields();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  const loginTab: NonNullable<TabsProps['items']>[number] = {
    key: 'login',
    label: 'Login',
    children: (
      <Form
        form={loginForm}
        layout="vertical"
        onFinish={handleLogin}
        style={{ marginTop: '1rem' }}
      >
        <EmailField />
        <PasswordField />
        <Form.Item>
          <Checkbox checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)}>
            Remember me
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
            Login
          </Button>
        </Form.Item>
        <Form.Item>
          <Button
            type="dashed"
            icon={<ThunderboltOutlined />}
            onClick={handleDevLogin}
            block
          >
            Quick Dev Login
          </Button>
        </Form.Item>
      </Form>
    ),
  };

  const registerTab: NonNullable<TabsProps['items']>[number] = {
    key: 'register',
    label: 'Register',
    children: (
      <Form
        form={registerForm}
        layout="vertical"
        onFinish={handleRegister}
        style={{ marginTop: '1rem' }}
      >
        <EmailField />
        <PasswordField />
        {/* Confirm password with cross-field validation */}
        <Form.Item
          label="Confirm Password"
          name="confirmPassword"
          dependencies={['password']}
          rules={[
            { required: true, message: 'Please confirm your password' },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue('password') === value) {
                  return Promise.resolve();
                }
                return Promise.reject(new Error('Passwords do not match'));
              },
            }),
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder="Repeat your password"
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
                  : Promise.reject(new Error('You must accept terms')),
            },
          ]}
          extra="By creating an account you agree to our terms and privacy policy."
        >
          <Checkbox>I accept the Terms and Privacy Policy</Checkbox>
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={loading} block>
            Create account
          </Button>
        </Form.Item>
      </Form>
    ),
  };

  return (
    <div style={{ maxWidth: 480, margin: '2rem auto' }}>
      <Typography.Title level={2} style={{ marginBottom: 0 }}>
        Welcome to TimeHacker
      </Typography.Title>
      <Typography.Text type="secondary">
        Sign in to access your smart calendar or create a new account.
      </Typography.Text>

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

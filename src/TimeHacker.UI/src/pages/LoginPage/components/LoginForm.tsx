import type { FC } from 'react';
import { Form, Input, Button, Checkbox } from 'antd';
import type { FormInstance } from 'antd';
import { MailOutlined, LockOutlined, LoginOutlined, ThunderboltOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import type { LoginFormData } from '../types';

const MIN_PASSWORD_LENGTH = 6;

interface LoginFormProps {
  form: FormInstance;
  loading: boolean;
  rememberMe: boolean;
  onRememberMeChange: (checked: boolean) => void;
  onFinish: (values: LoginFormData) => void;
  onDevLogin: () => void;
}

const LoginForm: FC<LoginFormProps> = ({ form, loading, rememberMe, onRememberMeChange, onFinish, onDevLogin }) => {
  const { t } = useTranslation();

  return (
    <Form form={form} layout="vertical" onFinish={onFinish} style={{ marginTop: '1rem' }}>
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

      <Form.Item
        label={t('login.password')}
        name="password"
        rules={[
          { required: true, message: t('login.passwordRequired') },
          { min: MIN_PASSWORD_LENGTH, message: t('login.passwordMinLength', { min: MIN_PASSWORD_LENGTH }) },
        ]}
        extra={t('login.passwordHint')}
      >
        <Input.Password prefix={<LockOutlined />} placeholder="••••••••" />
      </Form.Item>

      <Form.Item>
        <Checkbox checked={rememberMe} onChange={(e) => onRememberMeChange(e.target.checked)}>
          {t('login.rememberMe')}
        </Checkbox>
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" icon={<LoginOutlined />} loading={loading} block>
          {t('login.loginButton')}
        </Button>
      </Form.Item>

      <Form.Item>
        <Button type="dashed" icon={<ThunderboltOutlined />} onClick={onDevLogin} block>
          {t('login.devLogin')}
        </Button>
      </Form.Item>
    </Form>
  );
};

export default LoginForm;

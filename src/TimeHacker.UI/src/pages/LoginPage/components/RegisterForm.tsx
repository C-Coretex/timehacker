import type { FC } from 'react';
import { Form, Input, Button, Checkbox } from 'antd';
import type { FormInstance } from 'antd';
import { MailOutlined, LockOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import type { RegisterFormData } from '../types';

const MIN_PASSWORD_LENGTH = 6;

interface RegisterFormProps {
  form: FormInstance;
  loading: boolean;
  onFinish: (values: RegisterFormData) => void;
}

const RegisterForm: FC<RegisterFormProps> = ({ form, loading, onFinish }) => {
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
        <Input.Password prefix={<LockOutlined />} placeholder={t('login.repeatPasswordPlaceholder')} />
      </Form.Item>

      <Form.Item
        name="agree"
        valuePropName="checked"
        rules={[
          {
            validator: (_, value) =>
              value ? Promise.resolve() : Promise.reject(new Error(t('login.termsRequired'))),
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
  );
};

export default RegisterForm;

import type { FC } from 'react';
import { Button, Form, Input } from 'antd';
import type { FormInstance } from 'antd';
import { MailOutlined, PhoneOutlined, UserOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

export interface ProfileFormValues {
  name: string;
  emailForNotifications?: string;
  phoneNumberForNotifications?: string;
}

interface ProfileFormProps {
  form: FormInstance<ProfileFormValues>;
  saving: boolean;
  onCancel: () => void;
  onSave: () => void;
}

/** The editable profile fields, with Cancel / Save under them. */
export const ProfileForm: FC<ProfileFormProps> = ({ form, saving, onCancel, onSave }) => {
  const { t } = useTranslation();

  return (
    <Form form={form} layout="vertical" requiredMark={false} className="th-profile__form" onFinish={onSave}>
      <Form.Item label={t('profile.name')} name="name" rules={[{ required: true, message: t('profile.nameRequired') }]}>
        <Input prefix={<UserOutlined />} maxLength={64} />
      </Form.Item>
      <Form.Item
        label={t('profile.emailNotifications')}
        name="emailForNotifications"
        rules={[{ pattern: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/, message: t('profile.invalidEmail') }]}
      >
        <Input prefix={<MailOutlined />} type="email" />
      </Form.Item>
      <Form.Item label={t('profile.phoneNotifications')} name="phoneNumberForNotifications">
        <Input prefix={<PhoneOutlined />} />
      </Form.Item>
      <div className="th-profile__form-actions">
        <Button onClick={onCancel}>{t('profile.cancel')}</Button>
        <Button type="primary" htmlType="submit" loading={saving}>
          {t('profile.save')}
        </Button>
      </div>
    </Form>
  );
};

import { useEffect, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Form, Spin } from 'antd';
import { EditOutlined, MailOutlined, PhoneOutlined, UserOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { api } from 'api/api';
import { PageHeader } from 'components/PageHeader';
import { useAuth } from 'contexts/AuthContext';
import { getApiErrorMessage } from 'utils/getApiErrorMessage';
import { InfoRow } from './components/InfoRow';
import { ProfileForm } from './components/ProfileForm';
import type { ProfileFormValues } from './components/ProfileForm';
import { ProfileHero } from './components/ProfileHero';
import './styles.css';

/** The signed-in user's profile: a banner, and their contact details shown or edited in place. */
export const ProfilePage: FC = () => {
  const { user, fetchCurrentUser } = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form] = Form.useForm<ProfileFormValues>();
  const { t } = useTranslation();
  const { message } = App.useApp();

  useEffect(() => {
    fetchCurrentUser();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  if (!user) {
    return <Spin size="large" className="th-profile__loading" />;
  }

  const startEditing = () => {
    form.setFieldsValue({
      name: user.name,
      phoneNumberForNotifications: user.phoneNumberForNotifications,
      emailForNotifications: user.emailForNotifications,
    });
    setEditing(true);
  };

  const cancelEditing = () => {
    setEditing(false);
    form.resetFields();
  };

  const save = async () => {
    try {
      const values = form.getFieldsValue();
      setSaving(true);
      await api.put('/api/users/me', {
        name: values.name,
        phoneNumberForNotifications: values.phoneNumberForNotifications || null,
        emailForNotifications: values.emailForNotifications || null,
      });
      await fetchCurrentUser();
      setEditing(false);
      message.success(t('profile.profileUpdated'));
    } catch (err: unknown) {
      const msg = getApiErrorMessage(err);
      if (msg) message.error(msg);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="th-profile">
      <PageHeader title={t('profile.title')} />

      <ProfileHero
        name={user.name}
        email={user.emailForNotifications}
        action={
          !editing && (
            <Button icon={<EditOutlined />} onClick={startEditing}>
              {t('profile.editProfile')}
            </Button>
          )
        }
      />

      <section className="th-profile__card">
        <h2 className="th-profile__card-title">{t('profile.details')}</h2>
        {editing ? (
          <ProfileForm form={form} saving={saving} onCancel={cancelEditing} onSave={() => void save()} />
        ) : (
          <div className="th-profile__rows">
            <InfoRow icon={<UserOutlined />} label={t('profile.name')} value={user.name} />
            <InfoRow icon={<MailOutlined />} label={t('profile.emailNotifications')} value={user.emailForNotifications} />
            <InfoRow icon={<PhoneOutlined />} label={t('profile.phoneNotifications')} value={user.phoneNumberForNotifications} />
          </div>
        )}
      </section>
    </div>
  );
};

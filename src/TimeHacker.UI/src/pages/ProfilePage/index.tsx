import { useState, useEffect } from 'react';
import type { FC } from 'react';
import {
  Avatar, Button, Card, Divider, Form, Input, message, Spin, theme, Typography,
} from 'antd';
import {
  CloseOutlined, EditOutlined, MailOutlined, PhoneOutlined, SaveOutlined, UserOutlined,
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useAuth } from 'contexts/AuthContext';
import api from '../../api/api';
import InfoRow from './components/InfoRow';

const ProfilePage: FC = () => {
  const { user, fetchCurrentUser } = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form] = Form.useForm();
  const { token } = theme.useToken();
  const { t } = useTranslation();

  useEffect(() => {
    fetchCurrentUser();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  if (!user) {
    return <Spin size="large" style={{ display: 'block', margin: '2rem auto' }} />;
  }

  const initials = user.name
    ? user.name.split(' ').map((w) => w[0]).join('').toUpperCase().slice(0, 2)
    : '?';

  const handleEdit = () => {
    form.setFieldsValue({
      name: user.name,
      phoneNumberForNotifications: user.phoneNumberForNotifications,
      emailForNotifications: user.emailForNotifications,
    });
    setEditing(true);
  };

  const handleCancel = () => {
    setEditing(false);
    form.resetFields();
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      setSaving(true);
      await api.put('/api/User/Update', {
        name: values.name,
        phoneNumberForNotifications: values.phoneNumberForNotifications || null,
        emailForNotifications: values.emailForNotifications || null,
      });
      await fetchCurrentUser();
      setEditing(false);
      message.success(t('profile.profileUpdated'));
    } catch (err: unknown) {
      const msg = err && typeof err === 'object' && 'response' in err
        ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
        : null;
      if (msg) message.error(msg);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{ maxWidth: 560, margin: '0 auto', padding: '0 16px' }}>
      <Card>
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', paddingBottom: 8 }}>
          <Avatar size={80} style={{ backgroundColor: token.colorPrimary, fontSize: 32, fontWeight: 600 }}>
            {initials}
          </Avatar>
          <Typography.Title level={4} style={{ marginTop: 12, marginBottom: 0 }}>
            {user.name || t('profile.noNameSet')}
          </Typography.Title>
          {user.emailForNotifications && (
            <Typography.Text type="secondary">{user.emailForNotifications}</Typography.Text>
          )}
        </div>

        <Divider />

        {!editing ? (
          <>
            <InfoRow icon={<UserOutlined />} label={t('profile.name')} value={user.name} />
            <InfoRow icon={<MailOutlined />} label={t('profile.emailNotifications')} value={user.emailForNotifications} />
            <InfoRow icon={<PhoneOutlined />} label={t('profile.phoneNotifications')} value={user.phoneNumberForNotifications} />
            <Divider />
            <Button icon={<EditOutlined />} onClick={handleEdit} block>
              {t('profile.editProfile')}
            </Button>
          </>
        ) : (
          <>
            <Form form={form} layout="vertical">
              <Form.Item
                label={t('profile.name')}
                name="name"
                rules={[{ required: true, message: t('profile.nameRequired') }]}
              >
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
            </Form>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button icon={<CloseOutlined />} onClick={handleCancel} style={{ flex: 1 }}>
                {t('profile.cancel')}
              </Button>
              <Button type="primary" icon={<SaveOutlined />} onClick={handleSave} loading={saving} style={{ flex: 1 }}>
                {t('profile.save')}
              </Button>
            </div>
          </>
        )}
      </Card>
    </div>
  );
};

export default ProfilePage;

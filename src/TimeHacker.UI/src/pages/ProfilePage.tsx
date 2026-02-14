import { useState } from 'react';
import type { FC } from 'react';
import {
  Typography,
  Card,
  Button,
  Input,
  Form,
  Avatar,
  Divider,
  message,
  Spin,
  theme,
} from 'antd';
import {
  EditOutlined,
  SaveOutlined,
  CloseOutlined,
  UserOutlined,
  MailOutlined,
  PhoneOutlined,
} from '@ant-design/icons';
import { useAuth } from 'contexts/AuthContext';
import api from '../api/api';

const { Title, Text } = Typography;

const InfoRow: FC<{
  icon: React.ReactNode;
  label: string;
  value: string | undefined;
}> = ({ icon, label, value }) => (
  <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px 0' }}>
    <span style={{ fontSize: 18, opacity: 0.6 }}>{icon}</span>
    <div>
      <Text type="secondary" style={{ fontSize: 12 }}>
        {label}
      </Text>
      <br />
      <Text strong>{value || '—'}</Text>
    </div>
  </div>
);

const ProfilePage: FC = () => {
  const { user, fetchCurrentUser } = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form] = Form.useForm();
  const { token } = theme.useToken();

  if (!user) {
    return <Spin size="large" style={{ display: 'block', margin: '2rem auto' }} />;
  }

  const initials = user.name
    ? user.name
        .split(' ')
        .map((w) => w[0])
        .join('')
        .toUpperCase()
        .slice(0, 2)
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
      message.success('Profile updated');
    } catch (err: unknown) {
      const msg =
        err && typeof err === 'object' && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : null;
      if (msg) message.error(msg);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{ maxWidth: 560, margin: '0 auto' }}>
      <Card>
        <div
          style={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            paddingBottom: 8,
          }}
        >
          <Avatar
            size={80}
            style={{
              backgroundColor: token.colorPrimary,
              fontSize: 32,
              fontWeight: 600,
            }}
          >
            {initials}
          </Avatar>
          <Title level={4} style={{ marginTop: 12, marginBottom: 0 }}>
            {user.name || 'No name set'}
          </Title>
          {user.emailForNotifications && (
            <Text type="secondary">{user.emailForNotifications}</Text>
          )}
        </div>

        <Divider />

        {!editing ? (
          <>
            <InfoRow
              icon={<UserOutlined />}
              label="Name"
              value={user.name}
            />
            <InfoRow
              icon={<MailOutlined />}
              label="Email for notifications"
              value={user.emailForNotifications}
            />
            <InfoRow
              icon={<PhoneOutlined />}
              label="Phone for notifications"
              value={user.phoneNumberForNotifications}
            />
            <Divider />
            <Button icon={<EditOutlined />} onClick={handleEdit} block>
              Edit profile
            </Button>
          </>
        ) : (
          <>
            <Form form={form} layout="vertical">
              <Form.Item
                label="Name"
                name="name"
                rules={[{ required: true, message: 'Name is required' }]}
              >
                <Input prefix={<UserOutlined />} maxLength={64} />
              </Form.Item>
              <Form.Item
                label="Email for notifications"
                name="emailForNotifications"
                rules={[
                  {
                    pattern: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                    message: 'Please enter a valid email address',
                  },
                ]}
              >
                <Input prefix={<MailOutlined />} type="email" />
              </Form.Item>
              <Form.Item
                label="Phone for notifications"
                name="phoneNumberForNotifications"
              >
                <Input prefix={<PhoneOutlined />} />
              </Form.Item>
            </Form>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button
                icon={<CloseOutlined />}
                onClick={handleCancel}
                style={{ flex: 1 }}
              >
                Cancel
              </Button>
              <Button
                type="primary"
                icon={<SaveOutlined />}
                onClick={handleSave}
                loading={saving}
                style={{ flex: 1 }}
              >
                Save
              </Button>
            </div>
          </>
        )}
      </Card>
    </div>
  );
};

export default ProfilePage;

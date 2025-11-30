import { useMutation } from '@tanstack/react-query';
import { Card, Form, Input, Button, Typography } from 'antd';
import type { FC } from 'react';

import { loginUser } from 'api/auth';

import type { LoginRequestProps } from 'types/auth';

const { Title } = Typography;

export const LoginPage: FC = () => {
  const [form] = Form.useForm<LoginRequestProps>();

  const mutation = useMutation({
    mutationFn: loginUser,
    // onError: (error: any) => {
    // },
  });

  const onSubmit = (data: LoginRequestProps) => mutation.mutate(data);

  const isLoading = ['pending', 'loading'].includes(mutation.status);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <Card className="w-full max-w-sm shadow-lg rounded-xl">
        <Title level={3} className="text-center mb-6">
          Login
        </Title>
        <Form form={form} name="login" onFinish={onSubmit} layout="vertical">
          <Form.Item
            label="Email"
            name="email"
            rules={[
              { required: true, message: 'please enter your email' },
              { type: 'email', message: 'invalid email' },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Password"
            name="password"
            rules={[{ required: true, message: 'please enter your password' }]}
          >
            <Input.Password />
          </Form.Item>

          <Button type="primary" htmlType="submit" loading={isLoading}>
            Login
          </Button>
        </Form>
      </Card>
    </div>
  );
};

export default LoginPage;

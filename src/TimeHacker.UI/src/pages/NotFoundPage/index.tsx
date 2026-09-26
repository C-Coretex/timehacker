import type { FC } from 'react';
import { Button, Result } from 'antd';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export const NotFoundPage: FC = () => {
  const { t } = useTranslation();

  return (
    <Result
      status="404"
      title={t('notFound.title')}
      subTitle={t('notFound.subtitle')}
      extra={
        <Link to="/">
          <Button type="primary">{t('notFound.backHome')}</Button>
        </Link>
      }
    />
  );
};

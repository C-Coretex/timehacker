import type { FC } from 'react';
import { Tabs } from 'antd';
import { useTranslation } from 'react-i18next';

import { PageHeader } from '../../components/PageHeader';
import { FixedTasksTab } from './components/FixedTasksTab';
import { DynamicTasksTab } from './components/DynamicTasksTab';

export const TasksPage: FC = () => {
  const { t } = useTranslation();

  return (
    <div>
      <PageHeader title={t('tasks.allTasks')} />
      <Tabs
        items={[
          { key: 'fixed', label: t('tasks.fixedTasks'), children: <FixedTasksTab /> },
          { key: 'dynamic', label: t('tasks.dynamicTasks'), children: <DynamicTasksTab /> },
        ]}
      />
    </div>
  );
};

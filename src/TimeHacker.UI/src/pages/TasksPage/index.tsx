import type { FC } from 'react';
import { Tabs, Typography } from 'antd';
import { useTranslation } from 'react-i18next';

import { useIsMobile } from '../../hooks/useIsMobile';
import FixedTasksTab from './components/FixedTasksTab';
import DynamicTasksTab from './components/DynamicTasksTab';

const TasksPage: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();

  return (
    <div>
      <div style={{ marginBottom: '1rem' }}>
        <Typography.Title level={isMobile ? 4 : 2} style={{ margin: 0 }}>
          {t('tasks.allTasks')}
        </Typography.Title>
      </div>

      <Tabs
        items={[
          { key: 'fixed', label: t('tasks.fixedTasks'), children: <FixedTasksTab /> },
          { key: 'dynamic', label: t('tasks.dynamicTasks'), children: <DynamicTasksTab /> },
        ]}
      />
    </div>
  );
};

export default TasksPage;

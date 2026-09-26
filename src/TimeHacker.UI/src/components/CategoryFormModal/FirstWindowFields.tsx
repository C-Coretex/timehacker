import type { FC } from 'react';
import { Form, Switch } from 'antd';
import { useTranslation } from 'react-i18next';
import { WhenFields } from '../WhenFields';

/**
 * Create-only: the category's first time window, so a new category can land on the planner in one step.
 * Further windows are added from the category's row, as before.
 */
export const FirstWindowFields: FC = () => {
  const { t } = useTranslation();
  const withWindow = Form.useWatch<boolean | undefined>('withFirstWindow') ?? false;

  return (
    <div className="th-first-window">
      <label className="th-first-window__toggle">
        <Form.Item name="withFirstWindow" valuePropName="checked" initialValue={false} noStyle>
          <Switch size="small" />
        </Form.Item>
        {t('categoryForm.addFirstWindow')}
      </label>
      {withWindow && <WhenFields isEdit={false} />}
    </div>
  );
};

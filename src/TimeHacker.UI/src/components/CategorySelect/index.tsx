import { useEffect } from 'react';
import type { FC } from 'react';
import { Form, Select, Tag } from 'antd';
import { useTranslation } from 'react-i18next';

import { useCategories } from '../../hooks/useCategories';
import { argbToHex } from '../../utils/colorArgb';

/** Multi-select over the user's categories, bound to a form field holding category ids. */
export const CategorySelect: FC<{ name?: string; required?: boolean }> = ({
  name = 'categoryIds',
  required = false,
}) => {
  const { t } = useTranslation();
  const { categories, loading, fetchCategories } = useCategories();

  useEffect(() => {
    void fetchCategories();
  }, [fetchCategories]);

  return (
    <Form.Item
      name={name}
      label={t('taskForm.categories')}
      rules={required ? [{ required: true, message: t('taskForm.categoriesRequired') }] : undefined}
    >
      <Select
        mode="multiple"
        allowClear
        loading={loading}
        placeholder={t('taskForm.categoriesPlaceholder')}
        optionFilterProp="label"
        options={categories.map((category) => ({
          value: category.id,
          label: category.name,
        }))}
        tagRender={({ label, value, closable, onClose }) => {
          const category = categories.find((c) => c.id === value);
          return (
            <Tag color={category ? argbToHex(category.color) : undefined} closable={closable} onClose={onClose} style={{ marginInlineEnd: 4 }}>
              {label}
            </Tag>
          );
        }}
      />
    </Form.Item>
  );
};

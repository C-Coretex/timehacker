import type { FC } from 'react';
import { Tag, Tooltip } from 'antd';

import type { CategoryReturnModel } from '../../api/types';
import { argbToHex } from '../../utils/colorArgb';

/** The categories a task is linked to, as coloured tags that show the category's description on hover. */
export const CategoryTags: FC<{ categories: CategoryReturnModel[] }> = ({ categories }) => {
  if (categories.length === 0) return <>-</>;

  return (
    <>
      {categories.map((category) => (
        <Tooltip key={category.id} title={category.description || undefined}>
          <Tag color={argbToHex(category.color)} style={category.description ? { cursor: 'help' } : undefined}>
            {category.name}
          </Tag>
        </Tooltip>
      ))}
    </>
  );
};

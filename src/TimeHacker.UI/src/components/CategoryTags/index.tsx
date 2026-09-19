import type { FC } from 'react';
import { Tag } from 'antd';

import type { CategoryReturnModel } from '../../api/types';
import { argbToHex } from '../../utils/colorArgb';

/** The categories a task is linked to, as coloured tags. */
export const CategoryTags: FC<{ categories: CategoryReturnModel[] }> = ({ categories }) => {
  if (categories.length === 0) return <>-</>;

  return (
    <>
      {categories.map((category) => (
        <Tag key={category.id} color={argbToHex(category.color)}>
          {category.name}
        </Tag>
      ))}
    </>
  );
};

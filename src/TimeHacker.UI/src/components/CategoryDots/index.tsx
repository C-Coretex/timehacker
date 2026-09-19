import type { FC } from 'react';
import { Tooltip } from 'antd';

import type { CategoryReturnModel } from '../../api/types';
import { argbToHex } from '../../utils/colorArgb';

/**
 * A task's categories as colour circles, each naming itself on hover. Used where there is no room for
 * labels — a calendar event body, say.
 */
export const CategoryDots: FC<{ categories: CategoryReturnModel[]; size?: number }> = ({
  categories,
  size = 8,
}) => {
  if (categories.length === 0) return null;

  return (
    <span style={{ display: 'inline-flex', alignItems: 'center', gap: 3, verticalAlign: 'middle' }}>
      {categories.map((category) => (
        <Tooltip key={category.id} title={category.name}>
          <span
            aria-label={category.name}
            style={{
              width: size,
              height: size,
              borderRadius: '50%',
              backgroundColor: argbToHex(category.color),
              // A pale category would otherwise vanish into the event body.
              border: '1px solid rgba(0, 0, 0, 0.25)',
              boxSizing: 'border-box',
              flex: 'none',
            }}
          />
        </Tooltip>
      ))}
    </span>
  );
};

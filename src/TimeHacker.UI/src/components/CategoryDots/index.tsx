import type { CSSProperties, FC } from 'react';
import { Tooltip } from 'antd';
import type { CategoryReturnModel } from 'api/types';
import { argbToHex } from 'utils/colorArgb';
import './styles.css';

type DotCategory = Pick<CategoryReturnModel, 'id' | 'name' | 'description' | 'color'>;

/**
 * A task's categories as colour circles, each naming itself (and its description) on hover. Used where there
 * is no room for labels — a planner event, a list row.
 */
export const CategoryDots: FC<{ categories: DotCategory[] }> = ({ categories }) => {
  if (categories.length === 0) return null;

  return (
    <span className="th-category-dots">
      {categories.map((category) => (
        <Tooltip
          key={category.id}
          title={
            <>
              <strong>{category.name}</strong>
              {category.description && <div className="th-category-dots__description">{category.description}</div>}
            </>
          }
        >
          {/* An empty title stops the host event's native tooltip from stacking under this one. */}
          <span
            className="th-category-dot"
            role="img"
            aria-label={category.name}
            title=""
            style={{ '--th-dot-color': argbToHex(category.color) } as CSSProperties}
          />
        </Tooltip>
      ))}
    </span>
  );
};

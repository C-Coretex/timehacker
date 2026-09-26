import type { CSSProperties, FC } from 'react';
import { Button } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';
import { CategoryDots } from 'components/CategoryDots';
import type { ListCardProps } from './types';
import './styles.css';

/**
 * The design's list row — Today's tasks, and the task and category lists on phones: an accent rail, the title
 * with its category dots, an optional description and meta line, and a summary on the right. The row opens the
 * item; delete, when offered, is a separate button beside it.
 */
export const ListCard: FC<ListCardProps> = ({
  title,
  categories = [],
  description,
  meta,
  aside,
  accent,
  tone = 'fixed',
  onOpen,
  onDelete,
  deleteLabel,
  className,
  footer,
}) => (
  <div
    className={['th-list-card', `th-list-card--${tone}`, className].filter(Boolean).join(' ')}
    style={accent ? ({ '--th-list-card-accent': accent } as CSSProperties) : undefined}
  >
    <div className="th-list-card__row">
      <button type="button" className="th-list-card__open" onClick={onOpen}>
        <span className="th-list-card__main">
          <span className="th-list-card__heading">
            <span className="th-list-card__title">{title}</span>
            <CategoryDots categories={categories} />
          </span>
          {description && <span className="th-list-card__description">{description}</span>}
          {meta && <span className="th-list-card__meta">{meta}</span>}
        </span>
        {aside && <span className="th-list-card__aside">{aside}</span>}
      </button>
      {onDelete && (
        <Button
          type="text"
          danger
          shape="circle"
          className="th-list-card__delete"
          icon={<DeleteOutlined />}
          aria-label={deleteLabel}
          onClick={onDelete}
        />
      )}
    </div>
    {footer && <div className="th-list-card__footer">{footer}</div>}
  </div>
);

export { ListCardList } from './ListCardList';
export { ListCardRange } from './ListCardRange';

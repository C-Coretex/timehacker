import type { ReactNode } from 'react';
import { Empty, Spin } from 'antd';

interface ListCardListProps<T extends { id: string }> {
  items: T[];
  /** A spinner stands in for the list only until the first items arrive; a reload keeps the list on screen. */
  loading: boolean;
  emptyText: ReactNode;
  renderItem: (item: T) => ReactNode;
  className?: string;
}

/** A column of ListCards with its loading and empty states — Today's list and the phone lists of tasks and categories. */
export function ListCardList<T extends { id: string }>({ items, loading, emptyText, renderItem, className }: ListCardListProps<T>) {
  if (loading && items.length === 0) return <Spin className={['th-list-card-list__loading', className].filter(Boolean).join(' ')} />;
  if (items.length === 0) return <Empty className={className} description={emptyText} />;

  return (
    <ul className={['th-list-card-list', className].filter(Boolean).join(' ')}>
      {items.map((item) => (
        <li key={item.id}>{renderItem(item)}</li>
      ))}
    </ul>
  );
}

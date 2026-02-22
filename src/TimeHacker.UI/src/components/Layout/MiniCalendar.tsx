import { useMemo } from 'react';
import type { FC } from 'react';
import { Button, Calendar } from 'antd';
import {
  CalendarOutlined,
  DoubleLeftOutlined,
  DoubleRightOutlined,
  LeftOutlined,
  RightOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import type { CalendarView } from '../../contexts/CalendarDateContext';

interface Props {
  selectedDate: Date;
  onSelect: (date: Date) => void;
  currentView: CalendarView;
  weekStartDay: number;
}

const MiniCalendar: FC<Props> = ({ selectedDate, onSelect, currentView, weekStartDay }) => {
  const selected = dayjs(selectedDate);

  const rangeStart = useMemo(() => {
    switch (currentView) {
      case 'week': {
        const diff = (selected.day() - weekStartDay + 7) % 7;
        return selected.subtract(diff, 'day').startOf('day');
      }
      case '3day':
        return selected.startOf('day');
      case 'month':
        return selected.startOf('month');
      default:
        return selected.startOf('day');
    }
  }, [selected, currentView, weekStartDay]);

  const rangeEnd = useMemo(() => {
    switch (currentView) {
      case 'week':
        return rangeStart.add(6, 'day').endOf('day');
      case '3day':
        return selected.add(2, 'day').endOf('day');
      case 'month':
        return selected.endOf('month');
      default:
        return selected.endOf('day');
    }
  }, [selected, currentView, rangeStart]);

  return (
    <div className="mini-cal" style={{ padding: '4px 8px', borderTop: '1px solid rgba(255,255,255,0.1)' }}>
      <Calendar
        fullscreen={false}
        value={selected}
        onSelect={(date) => onSelect(date.toDate())}
        fullCellRender={(current, info) => {
          if (info.type !== 'date') return info.originNode;
          const inRange =
            currentView !== 'day' &&
            !current.isBefore(rangeStart, 'day') &&
            !current.isAfter(rangeEnd, 'day');
          if (!inRange) return info.originNode;
          return (
            <div style={{ background: 'rgba(22, 119, 255, 0.12)', borderRadius: 4 }}>
              {info.originNode}
            </div>
          );
        }}
        headerRender={({ value, onChange }) => (
          <div style={{ padding: '4px 0' }}>
            <div style={{ textAlign: 'center', fontWeight: 600, fontSize: 13, marginBottom: 4 }}>
              {value.format('MMMM YYYY')}
            </div>
            <div style={{ display: 'flex', justifyContent: 'center', gap: 4 }}>
              <Button
                type="text"
                size="small"
                icon={<DoubleLeftOutlined />}
                onClick={() => onChange(value.subtract(1, 'year'))}
                aria-label="Previous year"
              />
              <Button
                type="text"
                size="small"
                icon={<LeftOutlined />}
                onClick={() => onChange(value.subtract(1, 'month'))}
                aria-label="Previous month"
              />
              <Button
                type="text"
                size="small"
                onClick={() => onChange(dayjs())}
                aria-label="Today"
              >
                <CalendarOutlined />
              </Button>
              <Button
                type="text"
                size="small"
                icon={<RightOutlined />}
                onClick={() => onChange(value.add(1, 'month'))}
                aria-label="Next month"
              />
              <Button
                type="text"
                size="small"
                icon={<DoubleRightOutlined />}
                onClick={() => onChange(value.add(1, 'year'))}
                aria-label="Next year"
              />
            </div>
          </div>
        )}
      />
    </div>
  );
};

export default MiniCalendar;

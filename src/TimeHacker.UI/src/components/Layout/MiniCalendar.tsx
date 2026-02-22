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

interface Props {
  selectedDate: Date;
  onSelect: (date: Date) => void;
}

const MiniCalendar: FC<Props> = ({ selectedDate, onSelect }) => (
  <div className="mini-cal" style={{ padding: '4px 8px', borderTop: '1px solid rgba(255,255,255,0.1)' }}>
    <Calendar
      fullscreen={false}
      value={dayjs(selectedDate)}
      onSelect={(date) => onSelect(date.toDate())}
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

export default MiniCalendar;

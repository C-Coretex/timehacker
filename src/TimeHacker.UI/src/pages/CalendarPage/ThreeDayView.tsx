import type { FC } from 'react';
import { Navigate } from 'react-big-calendar';
import type { NavigateAction } from 'react-big-calendar';
// @ts-expect-error -- react-big-calendar internal module, no public types
import TimeGrid from 'react-big-calendar/lib/TimeGrid';

interface ThreeDayViewProps {
  date: Date;
  localizer: {
    format: (date: Date, format: string, culture?: string) => string;
    startOf: (date: Date, unit: string) => Date;
    endOf: (date: Date, unit: string) => Date;
  };
  min?: Date;
  max?: Date;
  scrollToTime?: Date;
  enableAutoScroll?: boolean;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}

const buildThreeDayRange = (date: Date): Date[] => {
  const start = new Date(date);
  start.setHours(0, 0, 0, 0);
  return Array.from({ length: 3 }, (_, i) => {
    const d = new Date(start);
    d.setDate(d.getDate() + i);
    return d;
  });
};

const ThreeDayView: FC<ThreeDayViewProps> & {
  range: (date: Date) => Date[];
  navigate: (date: Date, action: NavigateAction) => Date;
  title: (date: Date, extra: { localizer: ThreeDayViewProps['localizer'] }) => string;
} = (props) => {
  const {
    date,
    localizer,
    min = localizer.startOf(new Date(), 'day'),
    max = localizer.endOf(new Date(), 'day'),
    scrollToTime = localizer.startOf(new Date(), 'day'),
    enableAutoScroll = true,
    ...rest
  } = props;

  return (
    <TimeGrid
      {...rest}
      range={buildThreeDayRange(date)}
      eventOffset={15}
      localizer={localizer}
      date={date}
      min={min}
      max={max}
      scrollToTime={scrollToTime}
      enableAutoScroll={enableAutoScroll}
    />
  );
};

ThreeDayView.range = buildThreeDayRange;

ThreeDayView.navigate = (date: Date, action: NavigateAction): Date => {
  const newDate = new Date(date);
  switch (action) {
    case Navigate.PREVIOUS:
      newDate.setDate(newDate.getDate() - 3);
      break;
    case Navigate.NEXT:
      newDate.setDate(newDate.getDate() + 3);
      break;
    default:
      break;
  }
  return newDate;
};

ThreeDayView.title = (
  date: Date,
  { localizer }: { localizer: ThreeDayViewProps['localizer'] },
): string => {
  const end = new Date(date);
  end.setDate(end.getDate() + 2);
  return `${localizer.format(date, 'MMM DD')} – ${localizer.format(end, 'MMM DD')}`;
};

export default ThreeDayView;

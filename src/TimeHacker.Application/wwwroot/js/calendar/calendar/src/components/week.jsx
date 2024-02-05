import moment from 'moment'
import DayOfWeek from './dayOfWeek';
import { useState } from 'react';
import { v4 as uuid } from 'uuid';

function Week() {
  const [daysOfWeek, setdaysOfWeek] = useState(Array.from(['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'], (x, i) => {
    return { day: i + 1, dayName: x, isSelected: i + 1 == moment().day() }
  }));
  const [hoursOfDay, setHoursOfDay] = useState(Array.from({ length: 23 }, (_, i) => i + 1));

  return (
    <div className='tw-w-full tw-h-full tw-flex tw-justify-center'>
      <div className='tw-w-full tw-h-full tw-flex tw-flex-col tw-justify-center tw-items-center'>
        <div className='tw-pl-11 tw-w-full tw-h-fit tw-flex tw-flex-row tw-justify-center'>
          {
            daysOfWeek.map((d => <div key={uuid()} className='tw-w-[15%] tw-flex tw-justify-center'>{d.dayName}</div>))
          }
        </div>
        <div className='tw-w-full tw-h-[90%] tw-relative tw-border-black tw-border-2 tw-border-solid'>
          <Hours hoursOfDay={hoursOfDay} />

          <div className='tw-w-full tw-h-full tw-absolute tw-z-10'>
            <div className='tw-pl-11 tw-w-full tw-h-full tw-flex tw-flex-row'>
              {
                daysOfWeek.map((d => <DayOfWeek key={uuid()} day={d.day} isSelected={d.isSelected} />))
              }
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}

function Hours({ hoursOfDay }) {
  return <div className='tw-w-full tw-h-full tw-absolute tw-z-20'>
    <div className='tw-w-full tw-h-full tw-flex tw-flex-col tw-justify-between'>
      <div></div>
      {
        hoursOfDay.map((h) =>
          <div className='tw-w-full tw-h-[2px] tw-flex tw-flex-row tw-items-center' key={uuid()}>
            <div className='tw-ml-1 tw-mr-1'>{String(h).padStart(2, 0)}:00</div>
            <div className='tw-w-full tw-h-[2px] tw-bg-gray-400 tw-opacity-80' key={uuid()} />
          </div>
        )
      }
      <div></div>
    </div>
  </div>
}


export default Week;
import moment from 'moment'
import { useState } from 'react';
import { v4 as uuid } from 'uuid';

function DayOfWeek({ day, isSelected }) {
  const [hoursOfDay, setHoursOfDay] = useState(Array.from({ length: 23 }, (_, i) => i + 1));

  //console.log(hoursOfDay)

  return (
    <div className='tw-h-full tw-w-full'>
      <div className={`tw-h-full tw-w-full tw-border-black tw-border-0 tw-border-l-2 tw-border-solid ${isSelected ? 'tw-bg-slate-200' : ''}`}>
        <div className={`tw-h-full tw-flex tw-flex-col tw-justify-between tw-w-fit`}>
          {
            hoursOfDay.map((h) =>
              <div key={uuid()}>{<p>{/*h*/}</p>}</div>
            )
          }
        </div>
      </div>
    </div>
  );
}

export default DayOfWeek;
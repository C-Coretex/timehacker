import moment from 'moment'
import { useState } from 'react';
import { v4 as uuid } from 'uuid';

function DayOfWeek({ day, isSelected }) {
  const [hoursOfDay, setHoursOfDay] = useState(Array.from({ length: 23 }, (_, i) => i + 1));

  //console.log(hoursOfDay)

  return (
    <div className='h-full w-full'>
      <div className={`h-full w-full border-black border-l-2 ${isSelected ? 'bg-slate-200' : ''}`}>
        <div className={`h-full flex flex-col justify-between w-fit`}>
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
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
    <div className='w-full h-full flex justify-center'>
      <div className='w-full h-full flex flex-col justify-center items-center'>
        <div className='pl-11 w-full h-fit flex flex-row justify-center'>
          {
            daysOfWeek.map((d => <div key={uuid()} className='w-[15%] flex justify-center'>{d.dayName}</div>))
          }
        </div>
        <div className='w-full h-[90%] relative border-black border-2'>
          <Hours hoursOfDay={hoursOfDay} />

          <div className='w-full h-full absolute z-10'>
            <div className='pl-11 w-full h-full flex flex-row'>
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
  return <div className='w-full h-full absolute z-20'>
    <div className='w-full h-full flex flex-col justify-between'>
      <div></div>
      {
        hoursOfDay.map((h) =>
          <div className='w-full h-[2px] flex flex-row items-center' key={uuid()}>
            <div className='ml-1 mr-1'>{String(h).padStart(2, 0)}:00</div>
            <div className='w-full h-[2px] bg-gray-400 opacity-80' key={uuid()} />
          </div>
        )
      }
      <div></div>
    </div>
  </div>
}


export default Week;
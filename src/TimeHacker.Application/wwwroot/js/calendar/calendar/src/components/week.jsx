import moment from 'moment'
import DayOfWeek from './dayOfWeek';
import { useEffect, useRef, useState } from 'react';
import { v4 as uuid } from 'uuid';
import { API_URL, TEST_RESULT_FOR_DAY } from '../tools/variables'

function Week() {
  const hoursOfDay = Array.from({ length: 23 }, (_, i) => i + 1);

  const [mondayOfSelectedWeek, setMondayOfSelectedWeek] = useState(moment().startOf('isoWeek'))
  const [daysOfWeek, setdaysOfWeek] = useState([]);

  useEffect(() => {
    setDaysOfWeekCallback()
  }, [mondayOfSelectedWeek])


  function setDaysOfWeekCallback() {
    let days = Array.from(['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'], (x, i) => {
      let day = moment(mondayOfSelectedWeek).add(i, 'days')
      return { day: day, dayName: x, isSelected: day.isSame(moment(), 'day') }
    })


    getTasksForDays(days.map(d => d.day)).then((result) => {
      let tasks = result
      setdaysOfWeek(days.map((day, i) => {
        return { day: day.day, dayName: day.dayName, isSelected: day.isSelected, tasks: tasks[i] }
      }))
    }
    )
      .catch(error => {
        console.error(error);
      });
  }

  function changeSelectedWeek(isAddAction) {
    let addDaysCount = isAddAction ? 7 : -7;

    setMondayOfSelectedWeek(moment(mondayOfSelectedWeek).add(addDaysCount, 'days'))
  }

  function getTasksForDays(dates) {
    return fetch(`${API_URL}/Tasks/GetTasksForDays`,
      {
        method: "POST",
        body: JSON.stringify(dates),
        headers: new Headers({ 'content-type': 'application/json', 'accept': 'application/json' }),
      })
      .then(result => result.json())
      .then((result) => {
        return result
      },
        (error) => {
          console.log(error)
          throw error;
        })
  }

  return (
    <div className='tw-w-full tw-h-full tw-flex tw-justify-center'>
      <div className='tw-w-full tw-h-full tw-flex tw-flex-col tw-justify-center tw-items-center'>
        <div className='tw-flex tw-flex-row tw-w-full tw-h-fit'>
          <div className='tw-w-11 tw-flex tw-flex-row tw-items-center tw-justify-center'>
            <div
              className='tw-w-1/2 tw-h-fit hover:tw-cursor-pointer hover:*:tw-stroke-blue-500'
              onClick={() => changeSelectedWeek(false)}
            >
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                <path strokeLinecap="round" strokeLinejoin="round" d="m11.25 9-3 3m0 0 3 3m-3-3h7.5M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
              </svg>
            </div>

            <div
              className='tw-w-1/2 tw-h-fit hover:tw-cursor-pointer hover:*:tw-stroke-blue-500'
              onClick={() => changeSelectedWeek(true)}
            >
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                <path strokeLinecap="round" strokeLinejoin="round" d="m12.75 15 3-3m0 0-3-3m3 3h-7.5M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
              </svg>
            </div>
          </div>
          <div className='tw-w-[calc(100%-2.75rem)] tw-h-fit tw-flex tw-flex-row tw-justify-center'>
            {
              daysOfWeek.map((d => <div key={uuid()} className='tw-w-[15%] tw-p-2 tw-flex tw-justify-center tw-items-center tw-text-center tw-select-none'>{d.day.format('DD.MM.YYYY')} {d.dayName}</div>))
            }
          </div>
        </div>
        <div className='tw-w-full tw-h-full tw-relative tw-border-black tw-border-2 tw-border-solid'>
          <Hours hoursOfDay={hoursOfDay} />

          <div className='tw-w-full tw-h-full tw-absolute tw-z-10'>
            <div className='tw-pl-11 tw-w-full tw-h-full tw-flex tw-flex-row'>
              {
                daysOfWeek.map((d => <DayOfWeek key={uuid()} day={d.day} isSelected={d.isSelected} tasks={d.tasks} />))
              }
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}

function Hours({ hoursOfDay }) {
  return <div className='tw-w-full tw-h-full tw-absolute tw-z-20 tw-select-none tw-pointer-events-none'>
    <div className='tw-w-full tw-h-full tw-flex tw-flex-col tw-justify-between'>
      <div></div>
      {
        hoursOfDay.map((h) =>
          <div className='tw-w-full tw-h-[2px] tw-flex tw-flex-row tw-items-center' key={uuid()}>
            <div className='tw-ml-1 tw-mr-1 tw-select-none'>{String(h).padStart(2, 0)}:00</div>
            <div className='tw-w-full tw-h-[2px] tw-bg-gray-400 tw-opacity-40' key={uuid()} />
          </div>
        )
      }
      <div></div>
    </div>
  </div>
}


export default Week;
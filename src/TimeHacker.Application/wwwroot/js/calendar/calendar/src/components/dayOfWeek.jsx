import moment from 'moment'
import { useEffect, useState } from 'react';
import { v4 as uuid } from 'uuid';

function DayOfWeek({ day, isSelected, tasks }) {
  tasks = tasks.tasksTimeline
  const [taskElements, setTaskElements] = useState([])
  const countOfMinutesInDay = 1440

  function getMinutesFromTimestamp(timestamp) {
    const timeAtoms = timestamp.split(':')
    const timeInMinutes = parseInt(timeAtoms[0]) * 60 + parseInt(timeAtoms[1]) + parseInt(timeAtoms[2]) / 60

    return timeInMinutes
  }

  useEffect(() => {
    let taskElementsTemp = tasks.map((taskContainer) => {
      const startMinutes = getMinutesFromTimestamp(taskContainer.timeRange.start)
      const endMinutes = getMinutesFromTimestamp(taskContainer.timeRange.end)
      const taskTimelineInMinutes = endMinutes - startMinutes

      let height = countOfMinutesInDay / taskTimelineInMinutes

      return (
        <div className='tw-bg-red-700 tw-opacity-75 tw-outline tw-outline-1 tw-outline-black' style={{ height: `${height}%` }}>

        </div>
      )
    })

    setTaskElements(taskElementsTemp)
  }, [])

  return (
    <div className='tw-h-full tw-w-full tw-z-50'>
      <div className={`tw-h-full tw-w-full tw-border-black tw-border-0 tw-border-l-2 tw-border-solid ${isSelected ? 'tw-bg-slate-200' : ''}`}>
        <div className={`tw-h-full tw-flex tw-flex-col tw-justify-between tw-w-full`}>
          {
            taskElements
          }
        </div>
      </div>
    </div>
  );
}

export default DayOfWeek;
import moment from 'moment'
import React from 'react'
import { useEffect, useState } from 'react';
import { v4 as uuid } from 'uuid';
import { POSSIBLE_COLORS_FOR_TASK } from '../tools/variables'

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
    let lastTaskEndMinutes = 0

    let taskElementsTemp = tasks.map((taskContainer) => {
      const startMinutes = getMinutesFromTimestamp(taskContainer.timeRange.start)
      const endMinutes = getMinutesFromTimestamp(taskContainer.timeRange.end)
      const taskTimelineInMinutes = endMinutes - startMinutes

      let height = (100 / countOfMinutesInDay) * taskTimelineInMinutes
      let emptyTaskHeight = 0
      if(startMinutes > lastTaskEndMinutes)
        emptyTaskHeight = (100 / countOfMinutesInDay) * (startMinutes - lastTaskEndMinutes)

      lastTaskEndMinutes = endMinutes

      return (
      <React.Fragment key={uuid()}>
        <div style={{ height: `${emptyTaskHeight}%` }}>
          
        </div>
        <div className='tw-opacity-80 tw-flex tw-flex-row tw-items-center tw-justify-start tw-rounded-sm hover:tw-cursor-pointer p-1' style={{ height: `${height}%`, backgroundColor: POSSIBLE_COLORS_FOR_TASK[Math.floor(Math.random() * POSSIBLE_COLORS_FOR_TASK.length)] }}>
          <span className='tw-text-sm tw-pr-1'>{taskContainer.task.name}</span> <span className='tw-text-sm'>{(`${taskContainer.timeRange.start} - ${taskContainer.timeRange.end}`)}</span>
        </div>
      </React.Fragment>
      )
    })



    setTaskElements(taskElementsTemp)
  }, [])

  return (
    <div className='tw-h-full tw-w-full'>
      <div className={`tw-h-full tw-w-full tw-border-black tw-border-0 tw-border-l-2 tw-border-solid tw-opacity-95 ${isSelected ? 'tw-bg-green-300' : ''}`}>
        <div className={`tw-p-1 tw-h-full tw-flex tw-flex-col tw-justify-start tw-w-full`}>
          {
            taskElements
          }
        </div>
      </div>
    </div>
  );
}

export default DayOfWeek;
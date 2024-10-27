const baseUrl = window.location.origin + '/api'

const fixedTaskStartTimestamp = $("#fixedTaskStartTimestamp").flatpickr({
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    static: true,
    locale: {
        firstDayOfWeek: 1
    }
});

const fixedTaskEndTimestamp = $("#fixedTaskEndTimestamp").flatpickr({
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    static: true,
    locale: {
        firstDayOfWeek: 1
    }
});

const dynamicTaskMinTimeToFinish = $("#dynamicTaskMinTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

const dynamicTaskMaxTimeToFinish = $("#dynamicTaskMaxTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

const dynamicTaskOptimalTimeToFinish = $("#dynamicTaskOptimalTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

$('.js-open-fixed-tasks-button').on('click', () => {
    $('#openFixedTasksLi').addClass('active')
    $('#openDynamicTasksLi').removeClass('active')

    $('.js-fixed-tasks').removeClass('d-none')
    $('.js-dynamic-tasks').addClass('d-none')
})

$('.js-open-dynamic-tasks-button').on('click', () => {
    $('#openDynamicTasksLi').addClass('active')
    $('#openFixedTasksLi').removeClass('active')

    $('.js-dynamic-tasks').removeClass('d-none')
    $('.js-fixed-tasks').addClass('d-none')
})

function getDatesForCurrentWeek() {
    const dates = [];
    const today = new Date();

    // Get the current day of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
    const dayOfWeek = today.getDay();

    // Calculate the date of the Monday of the current week
    const monday = new Date(today);
    monday.setDate(today.getDate() - (dayOfWeek === 0 ? 6 : dayOfWeek - 1));

    // Add each day of the week to the dates array
    for (let i = 0; i < 7; i++) {
        const currentDay = new Date(monday);
        currentDay.setDate(monday.getDate() + i);
        dates.push(currentDay.toISOString().split('T')[0]); // Save the date as 'YYYY-MM-DD'
    }

    return dates;
}

$('.js-refresh').on('click', () => {
    console.log('aa')
    const thisWeekDates = getDatesForCurrentWeek();

    // Make the AJAX POST request
    $.ajax({
        url: '/api/Tasks/RefreshTasksForDays',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(thisWeekDates),
        success: (response) => {
            console.log('Tasks refreshed successfully:', response);
            // Handle success (e.g., update the UI)
        },
        error: (xhr, status, error) => {
            console.error('Error refreshing tasks:', status, error);
            // Handle error (e.g., show an error message)
        }
    });
})

$('.js-button-edit-fixed-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')
    const editFixedTaskModal = $("#editFixedTaskModal");

    fetch(`${baseUrl}/tasks/getfixedtaskbyid?id=${taskId}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .then(response => {
            setValueToFirstChild(editFixedTaskModal, '#fixedTaskId', taskId)
            setValueToFirstChild(editFixedTaskModal, '#fixedTaskName', response.name)
            setValueToFirstChild(editFixedTaskModal, '#fixedTaskDescription', response.description)
            setValueToFirstChild(editFixedTaskModal, '#fixedTaskCategory', response.category)
            setValueToFirstChild(editFixedTaskModal, '#fixedTaskPriority', response.priority)
            fixedTaskStartTimestamp.setDate(new Date(response.startTimestamp))
            fixedTaskEndTimestamp.setDate(new Date(response.endTimestamp))
        })
        .catch(error => console.log(error))
})

$('.js-button-delete-fixed-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')

    fetch(`${baseUrl}/tasks/deletefixedtask?id=${taskId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                $(taskContainer).remove()
            }
        })
        .catch(error => console.log(error))
})


$('.js-button-edit-dynamic-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')

    const editDynamicTaskModal = $("#editDynamicTaskModal");

    fetch(`${baseUrl}/tasks/getdynamictaskbyid?id=${taskId}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .then(response => {
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskId', taskId)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskName', response.name)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskDescription', response.description)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskCategory', response.category)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskPriority', response.priority)
            setTimeFlatpickr(dynamicTaskMinTimeToFinish, response.minTimeToFinish)
            setTimeFlatpickr(dynamicTaskMaxTimeToFinish, response.maxTimeToFinish)
            setTimeFlatpickr(dynamicTaskOptimalTimeToFinish, response.optimalTimeToFinish)
        })
        .catch(error => console.log(error))
})

function setTimeFlatpickr(flatpickrInstance, time) {
    const timeArray = time.split(':')
    const date = new Date()
    date.setHours(timeArray[0])
    date.setMinutes(timeArray[1])
    flatpickrInstance.setDate(date)
}

$('.js-button-delete-dynamic-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')

    fetch(`${baseUrl}/tasks/deletedynamictask?id=${taskId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                $(taskContainer).remove()
            }
        })
        .catch(error => console.log(error))
})
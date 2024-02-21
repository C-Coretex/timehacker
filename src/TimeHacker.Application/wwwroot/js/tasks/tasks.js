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
    altInput: true,
    static: true
});

const dynamicTaskMaxTimeToFinish = $("#dynamicTaskMaxTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    altInput: true,
    static: true
});

const dynamicTaskOptimalTimeToFinish = $("#dynamicTaskOptimalTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    altInput: true,
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
            console.log(response)

            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskName', response.name)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskDescription', response.description)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskCategory', response.category)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskPriority', response.priority)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskMinTimeToFinish', response.minTimeToFinish)

            console.log(response.maxTimeToFinish)
            const test = response.maxTimeToFinish.split(':')
            const date = new Date()
            date.setHours(test[0])
            date.setMinutes(test[1])
            dynamicTaskMaxTimeToFinish.setDate(date)
            //setValueToFIrstChild(editDynamicTaskModal, '#dynamicTaskMaxTimeToFinish', response.maxTImeToFinish)
            setValueToFirstChild(editDynamicTaskModal, '#dynamicTaskOptimalTimeToFinish', response.optimalTimeToFinish)
        })
        .catch(error => console.log(error))
})

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
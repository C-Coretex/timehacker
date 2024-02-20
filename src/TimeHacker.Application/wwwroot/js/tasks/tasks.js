const baseUrl = window.location.origin + '/api'

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
        console.log(response)
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
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
    console.log(taskId)
})

$('.js-button-delete-fixed-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')
    console.log(taskId)
})


$('.js-button-edit-dynamic-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')
    console.log(taskId)
})

$('.js-button-delete-dynamic-task').on('click', function () {
    const self = this
    const taskContainer = $(this).closest('.task-container')
    const taskId = $(taskContainer).attr('item-id')
    console.log(taskId)
})
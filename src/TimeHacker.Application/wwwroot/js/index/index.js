$("#fixedTaskStartTimestamp").flatpickr({
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    static: true,
    locale: {
        firstDayOfWeek: 1
    }
});

$("#fixedTaskEndTimestamp").flatpickr({
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    static: true,
    locale: {
        firstDayOfWeek: 1
    }
});

$("#dynamicTaskMinTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

$("#dynamicTaskMaxTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

$("#dynamicTaskOptimalTimeToFinish").flatpickr({
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: true,
    defaultDate: "00:00",
    static: true
});

$(".js-add-fixed-task-button").on("click", () => {
    $("#addFixedTaskLi").addClass("active");
    $("#addDynamicTaskLi").removeClass("active");

    $("#fixedTaskModal").removeClass("d-none");
    $("#dynamicTaskModal").addClass("d-none");
});

$(".js-add-dynamic-task-button").on("click", () => {
    $("#addDynamicTaskLi").addClass("active");
    $("#addFixedTaskLi").removeClass("active");

    $("#dynamicTaskModal").removeClass("d-none");
    $("#fixedTaskModal").addClass("d-none");
});
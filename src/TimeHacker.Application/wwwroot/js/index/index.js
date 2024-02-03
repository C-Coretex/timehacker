console.log("aaaa");

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

$(".js-add-dynamic-task-button").on("click", function () {
    console.log("clicked1");

    //$("#fixed-task-container").removeAttr("hidden");
})

$(".js-add-fixed-task-button").on("click", function () {
    console.log("clicked2");
    //$("#fixed-task-container").removeAttr("hidden");
})
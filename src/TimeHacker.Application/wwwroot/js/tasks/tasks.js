$(".js-open-fixed-tasks-button").on("click", () => {
    $("#openFixedTasksLi").addClass("active");
    $("#openDynamicTasksLi").removeClass("active");

    $(".js-fixed-tasks").removeClass("d-none");
    $("js-dynamic-tasks").addClass("d-none");
});

$(".js-open-dynamic-tasks-button").on("click", () => {
    $("#openDynamicTasksLi").addClass("active");
    $("#openFixedTasksLi").removeClass("active");

    $("js-dynamic-tasks").removeClass("d-none");
    $(".js-fixed-tasks").addClass("d-none");
});
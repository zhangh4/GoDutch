$(function () {

    $("[data-hide]").on("click", function () {
        //                $("." + $(this).attr("data-hide")).hide();
        // -or-, see below
        $(this).closest("." + $(this).attr("data-hide")).hide();
    });

    $.ajaxSetup({
        contentType: "application/json; charset=utf-8"
    });

    function getWebRoot() {
        var webRoot = '';
        return webRoot ? '/' + webRoot : '';
    }

    $("#createEventButton").click(function () {
        viewModel.editMode(true);
        $("#eventName").focus();
    });

    //            $("#saveEventButton").click(function () {
    $("#addEventForm").submit(function (e) {
        e.preventDefault();

        var event = { name: viewModel.eventName };

        viewModel.isSubmitting(true);

        $.post(getWebRoot() + '/api/events', ko.toJSON(event))
        .done(function (result) {
            window.location.href = getWebRoot() + '/events/' + result.id;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForEvent").show();
            viewModel.hasValidationError(true);
            viewModel.isSubmitting(false);
        });
    });

    $("#cancelEventButton").click(function (e) {
        e.preventDefault();
        viewModel.editMode(false);
    });

    var viewModel = {
        editMode: ko.observable(false),
        eventName: ko.observable(),
        isSubmitting: ko.observable(),
        hasValidationError: ko.observable(false),
        clearValidationError: function () { this.hasValidationError(false); },
        events: ko.observableArray(),
        gotoEvent: function (event) { window.location.href = getWebRoot() + '/events/' + event.id; },
        eventToRemove: ko.observable(),
        rememberEventToRemove: function(event) {
             this.eventToRemove(event);
        },
        removeEvent: function () {
            var e = this.eventToRemove();
            var that = this;
            $.ajax({
                url: getWebRoot() + '/api/events?id=' + e.id,
                type: 'DELETE'
            })
                .done(function () {
                    that.events.remove(e);
                });
        }
    };

    viewModel.isSubmitting.subscribe(function (newValue) {
        if (newValue === true) $("#saveEventButton").button('loading');
        if (newValue === false) $("#saveEventButton").button('reset');
    });

    ko.applyBindings(viewModel);

//    $.getJSON(getWebRoot() + "/api/families", function (data) {
//        overall.families(data);
//    });

    $.getJSON(getWebRoot() + "/api/events", function (data) {
        viewModel.events(data);
    });

});


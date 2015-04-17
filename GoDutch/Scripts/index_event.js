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
    });

    //            $("#saveEventButton").click(function () {
    $("#editEventForm").submit(function (e) {
        e.preventDefault();

        var event = viewModel.event();
        var events = viewModel.overall.events;

        if (event.equals(viewModel.eventBeforeEdit)) {
            viewModel.editMode(false);
            return;
        }

        event.isSubmitting(true);
        if (event.id) {
            $.ajax({
                url: getWebRoot() + '/api/events?id=' + event.id,
                data: ko.toJSON(event),
                type: 'PUT'
            })
            .done(function () {
                events.splice(
                    events().indexOf(
                        events().filter(
                            function (e) { return e.id === event.id; })[0]),
                    1,
                    event);
                viewModel.editMode(false);
                event.isSubmitting(false);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForEvent").show();
                event.hasValidationError(true);
                event.isSubmitting(false);
            });
        } else {
            $.post(getWebRoot() + '/api/events', ko.toJSON(event))
            .done(function (result) {
                event.id = result.id;
                events.unshift(event);
                viewModel.editMode(false);
                event.isSubmitting(false);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForEvent").show();
                event.hasValidationError(true);
                event.isSubmitting(false);
            });
        }
    });

    $("#cancelEventButton").click(function (e) {
        e.preventDefault();
        viewModel.editMode(false);
    });

    $("#confirmCancelEventButton").click(function (e) {
        //                e.preventDefault();
        viewModel.editMode(false);
    });

    var viewModel = {
        editMode: ko.observable(false),
        eventName: ko.observable()
    };

    ko.applyBindings(viewModel);

//    $.getJSON(getWebRoot() + "/api/families", function (data) {
//        overall.families(data);
//    });

//    $.getJSON(getWebRoot() + "/api/events", function (data) {
//        overall.events(data.map(function (e) { return new Event(e); }));

//    });

});


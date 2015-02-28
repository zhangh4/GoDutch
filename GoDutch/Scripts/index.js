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
        viewModel.event(viewModel.createNewEvent());
        viewModel.editMode(true);
    });

    //            $("#saveEventButton").click(function () {
    $("#editEventForm").submit(function (e) {
        e.preventDefault();

        var event = viewModel.event();
        var events = viewModel.overall.events;
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
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForEvent").show();
                event.hasValidationError(true);
            });
        } else {
            $.post(getWebRoot() + '/api/events', ko.toJSON(event))
            .done(function (result) {
                event.id = result.id;
                events.unshift(event);
                viewModel.editMode(false);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForEvent").show();
                event.hasValidationError(true);
            });
        }
    });

    $("#cancelEventButton").click(function (e) {
        e.preventDefault();
        //                viewModel.editMode(false);
    });

    $("#confirmCancelEventButton").click(function (e) {
        //                e.preventDefault();
        viewModel.editMode(false);
    });

    function Event(spec) {
        this.hasValidationError = ko.observable(false);
        this.id = spec && spec.id;
        this.name = spec && ko.observable(spec.name);
        this.attendingFamilies = spec && spec.attendingFamilies.map(function (f) { return new Family(f, this); }, this);
        this.trulyAttendingFamilies = function () {
            return this.attendingFamilies.filter(function (f) { return f.expense() || f.count(); });
        }

        this.getTotalExpenses = function () {
            return this.attendingFamilies
                .map(function (f) { return f.expense() || 0; })
                .reduce(function (total, current) { return total + current; }, 0);
        };
        this.getTotalCount = function () {
            return this.attendingFamilies
                .map(function (f) { return f.count() || 0; })
                .reduce(function (total, current) { return total + current; }, 0);
        };
        this.getPaymentPerPerson = function () {
            var totalCount = this.getTotalCount();
            return totalCount === 0 ? null : this.getTotalExpenses() / totalCount;
        };

        function Family(spec, parent) {
            this.id = spec.id;
            this.name = spec.name;
            this.expense = ko.numericObservable(spec.expense);
            this.count = ko.numericObservable(spec.count);
            this.getPayment = function () { return parent.getPaymentPerPerson() * this.count(); };
            this.getBalance = function () {
                return Math.round10(this.expense() - this.getPayment(), -2).toFixed(2);
            };
        }
    }

    var overall = {
        families: ko.observableArray(),//[ { id: 1, name: 'Brayden' }, { id: 2, name: 'Jason' }, { id: 3, name: 'Debra' } ],
        events: ko.observableArray(),
        eventToRemove: ko.observable(),
        getTotalBalances: function () {
            return this.families()
                .map(function (f) {
                    return this.events()
                        .map(function (e) {
                            return e.attendingFamilies
                                .filter(function (ff) {
                                    return ff.id === f.id;
                                })[0].getBalance();
                        })
                        .reduce(function (total, balance) {
                            return total + balance;
                        }, 0);
                }, this)
                .map(function (v) { return Math.round10(v, -2); });
        },
        rememberEventToRemove: function (e) {
            this.eventToRemove(e);
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
        },
        editEvent: function (e) {
            var eventInJS = ko.toJS(e);
            var clone = new Event({ id: eventInJS.id, name: eventInJS.name, attendingFamilies: eventInJS.attendingFamilies });
            viewModel.event(clone);
            viewModel.editMode(true);
        }
    };

    var viewModel = {
        overall: overall,
        event: ko.observable(),
        editMode: ko.observable(false),
        createNewEvent: function () {
            return new Event({
                name: '',
                attendingFamilies: overall.families().map(function (f) { return { id: f.id, name: f.name, expense: null, count: null }; })
            });
        },
        allowNumbersOnly: function (data, e) {
            alert(e);
            var key = e.keyCode ? e.keyCode : e.which;
            if (isNaN(String.fromCharCode(key))) return false;
            return true;
        }
    };
    viewModel.event(viewModel.createNewEvent());

    ko.applyBindings(viewModel);
    //            ko.applyBindings(overall, $('#Overall')[0]);
    //            ko.applyBindings(event, $('#createEvent')[0]);

    $.getJSON(getWebRoot() + "/api/families", function (data) {
        overall.families(data);
    });

    $.getJSON(getWebRoot() + "/api/events", function (data) {
        overall.events(data.map(function (e) { return new Event(e); }));
    });

});

function alpha(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return (k >= 48 && k <= 57) || k == 46;
}
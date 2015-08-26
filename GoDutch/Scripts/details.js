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

    $("#createExpenseButton").click(function () {
        var defaultExpenseName = "费用 " + (viewModel.overall.expenses().length + 1);
        viewModel.expense(viewModel.createNewExpense(defaultExpenseName));
        viewModel.expenseBeforeEdit = viewModel.createNewExpense(defaultExpenseName);
        viewModel.editMode(true);
        $("#expenseName").focus();

    });

    //            $("#saveExpenseButton").click(function () {
    $("#editExpenseForm").submit(function (e) {
        e.preventDefault();

        var expense = viewModel.expense();
        var expenses = viewModel.overall.expenses;

        if (expense.equals(viewModel.expenseBeforeEdit)) {
            viewModel.editMode(false);
            return;
        }

        var index = expenses().indexOf(viewModel.expenseBeforeEdit); // must use the original array to do object reference lookup
        var event = viewModel.overall.createEvent();
        expense.isSubmitting(true);
        if (index < 0) {
            event.expenses.unshift(expense);
            $.post(getWebRoot() + '/api/events', ko.toJSON(event))
                .done(function (result) {
                    expenses.unshift(expense);
                    viewModel.editMode(false);
                    expense.isSubmitting(false);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    //                $("#errorMessageForExpense").show();
                    expense.hasValidationError(true);
                    expense.isSubmitting(false);
                });
        }
        else {
            event.expenses.splice(index, 1, expense);
            $.ajax({
                    url: getWebRoot() + '/api/events?id=' + event.id,
                    data: ko.toJSON(event),
                    type: 'PUT'
                })
                .done(function () {
                    expenses.splice(index, 1, expense);
                    viewModel.editMode(false);
                    expense.isSubmitting(false);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    //                $("#errorMessageForExpense").show();
                    expense.hasValidationError(true);
                    expense.isSubmitting(false);
                });
        }
    });

    $("#cancelExpenseButton").click(function (e) {
        e.preventDefault();
        if (!viewModel.expense().equals(viewModel.expenseBeforeEdit)) {
            $("#confirmCancelEdit").modal('show');
        } else {
            viewModel.editMode(false);
        }
        //                viewModel.editMode(false);
    });

    $("#confirmCancelExpenseButton").click(function (e) {
        //                e.preventDefault();
        viewModel.editMode(false);
    });

    function Event(spec) {
        this.id = spec.id;
        this.name = spec.name;
        this.expenses = spec.expenses;
    }

    function Expense(spec) {
        this.hasValidationError = ko.observable(false);
        this.clearValidationError = function () { this.hasValidationError(false); }
        this.isSubmitting = ko.observable(false);
        this.isSubmitting.subscribe(function(newValue) {
            if (newValue === true) $("#saveExpenseButton").button('loading');
            if (newValue === false) $("#saveExpenseButton").button('reset');
        });
        this.id = spec && spec.id;
        this.name = spec && ko.observable(spec.name);
        this.eventId = spec && spec.eventId;
        this.attendingFamilies = spec && spec.attendingFamilies.map(function (f) { return new Family(f, this); }, this);
        this.trulyAttendingFamilies = function () {
            return this.attendingFamilies.filter(function (f) { return f.participated(); });
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
        this.equals = function (other) {
            return ko.toJSON(this) === ko.toJSON(other);
        }

        function Family(spec, parent) {
            this.id = spec.id;
            this.name = spec.name;
            this.expense = ko.numericObservable(spec.expense);
            this.count = ko.numericObservable(spec.count);
            this.getPayment = function () { return parent.getPaymentPerPerson() * this.count(); };
            this.getBalanceInNumeric = function() {
                return Math.round10(this.expense() - this.getPayment(), -2);
            };
            this.getBalance = function () {
                return this.getBalanceInNumeric().toFixed(2);
            };
            this.getBalanceNullable = function() {
                return this.participated() ? this.getBalance() : null;
            };
            this.participated = function() { return this.expense() || this.count(); };
        }
    }

    var overall = {
        families: ko.observableArray(),//[ { id: 1, name: 'Brayden' }, { id: 2, name: 'Jason' }, { id: 3, name: 'Debra' } ],
        expenses: ko.observableArray(),
        expenseToRemove: ko.observable(),
        eventId: eventId,
        eventName: ko.observable(),
        loading: ko.observable(true),
        createEvent: function() {
            return new Event({ id: this.eventId, name: this.eventName(), expenses: ko.toJS(this.expenses) });
        },
        getTotalBalances: function () {
            return this.families()
                .map(function (f) {
                    return this.expenses()
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
        getGrandTotal: function () {
            return _.chain(overall.expenses())
                    .reduce(function (total, current) {
                                 total
                                    .push
                                    .apply(total, current.trulyAttendingFamilies());
                                return total;
                            }, [])
                    .groupBy('name')
                    .pairs()
                    .map(function(namePlusFamilies) {
                        return {
                            name: namePlusFamilies[0],
                            total: namePlusFamilies[1].reduce(function (total, current) {
                                return total + current.getBalanceInNumeric();
                            }, 0)
                        };
                    })
                    .value();
        },
        rememberExpenseToRemove: function (e) {
            this.expenseToRemove(e);
        },
        removeExpense: function () {
            var e = this.expenseToRemove();
//            var index = this.expenses().indexOf(e);
            var event = this.createEvent();
            event.expenses.splice(event.expenses.indexOf(e), 1);
            var that = this;
            $.ajax({
                url: getWebRoot() + '/api/events?id=' + that.eventId,
                data: ko.toJSON(event),
                type: 'PUT'
            })
                .done(function () {
                    that.expenses.remove(e);
                });
        },
        editExpense: function (e) {
            var expenseInJS = ko.toJS(e);
            viewModel.expenseBeforeEdit = e;
            var clone = new Expense({ id: expenseInJS.id, name: expenseInJS.name, attendingFamilies: expenseInJS.attendingFamilies, eventId: expenseInJS.eventId });
            viewModel.expense(clone);
//            viewModel.expense(expenseInJS);
            viewModel.editMode(true);
            $("#expenseName").focus();
        }
    };

    var viewModel = {
        overall: overall,
        expense: ko.observable(),
        editMode: ko.observable(false),
        expenseBeforeEdit: null,
        createNewExpense: function (defaultName) {
            return new Expense({
                name: defaultName,
                eventId: overall.eventId,
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

    viewModel.expense(viewModel.createNewExpense());

    ko.applyBindings(viewModel);
    //            ko.applyBindings(overall, $('#Overall')[0]);
    //            ko.applyBindings(expense, $('#createExpense')[0]);

    $.when(
        $.getJSON(getWebRoot() + "/api/families", function (data) {
            overall.families(data);
        }),
        $.getJSON(getWebRoot() + "/api/events/" + overall.eventId, function (event) {
            overall.expenses(event.expenses.map(function (e) { return new Expense(e); }));
            overall.eventName(event.name);
        })
     ).done(function() {
        viewModel.overall.loading(false);
    });

    

});

function alpha(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return (k >= 48 && k <= 57) || k == 46;
}
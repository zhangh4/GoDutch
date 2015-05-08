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

        expense.isSubmitting(true);
        if (expense.id) {
            $.ajax({
                url: getWebRoot() + '/api/expenses?id=' + expense.id,
                data: ko.toJSON(expense),
                type: 'PUT'
            })
            .done(function () {
                expenses.splice(
                    expenses().indexOf(
                        expenses().filter(
                            function (e) { return e.id === expense.id; })[0]),
                    1,
                    expense);
                viewModel.editMode(false);
                expense.isSubmitting(false);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
//                $("#errorMessageForExpense").show();
                expense.hasValidationError(true);
                expense.isSubmitting(false);
            });
        } else {
            $.post(getWebRoot() + '/api/expenses', ko.toJSON(expense))
            .done(function (result) {
                expense.id = result.id;
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

    function Expense(spec) {
        this.hasValidationError = ko.observable(false);
        this.clearValidationError = function () { this.hasValidationError(false); }
        this.isSubmitting = ko.observable();
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
            var that = this;
            $.ajax({
                url: getWebRoot() + '/api/expenses?id=' + e.id,
                type: 'DELETE'
            })
                .done(function () {
                    that.expenses.remove(e);
                });
        },
        editExpense: function (e) {
            var expenseInJS = ko.toJS(e);
            viewModel.expenseBeforeEdit = expenseInJS;
            var clone = new Expense({ id: expenseInJS.id, name: expenseInJS.name, attendingFamilies: expenseInJS.attendingFamilies, eventId: expenseInJS.eventId });
            viewModel.expense(clone);
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

    $.getJSON(getWebRoot() + "/api/families", function (data) {
        overall.families(data);
    });

    $.getJSON(getWebRoot() + "/api/events/" + overall.eventId, function (event) {
        overall.expenses(event.expenses.map(function (e) { return new Expense(e); }));
        overall.eventName(event.name);
//        alert(viewModel.overall.getGrandTotal());
    });

});

function alpha(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return (k >= 48 && k <= 57) || k == 46;
}
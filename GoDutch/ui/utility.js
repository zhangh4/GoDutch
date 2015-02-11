// Closure
(function(){

    /**
     * Decimal adjustment of a number.
     *
     * @param	{String}	type	The type of adjustment.
     * @param	{Number}	value	The number.
     * @param	{Integer}	exp		The exponent (the 10 logarithm of the adjustment base).
     * @returns	{Number}			The adjusted value.
     */
    function decimalAdjust(type, value, exp) {
        // If the exp is undefined or zero...
        if (typeof exp === 'undefined' || +exp === 0) {
            return Math[type](value);
        }
        value = +value;
        exp = +exp;
        // If the value is not a number or the exp is not an integer...
        if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
            return NaN;
        }
        // Shift
        value = value.toString().split('e');
        value = Math[type](+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp)));
        // Shift back
        value = value.toString().split('e');
        return +(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp));
    }

    // Decimal round
    if (!Math.round10) {
        Math.round10 = function(value, exp) {
            return decimalAdjust('round', value, exp);
        };
    }
    // Decimal floor
    if (!Math.floor10) {
        Math.floor10 = function(value, exp) {
            return decimalAdjust('floor', value, exp);
        };
    }
    // Decimal ceil
    if (!Math.ceil10) {
        Math.ceil10 = function(value, exp) {
            return decimalAdjust('ceil', value, exp);
        };
    }

})();

ko.numericObservable = function(initialValue) {
    var _actual = ko.observable(initialValue);

    var result = ko.dependentObservable({
        read: function() {
            return _actual();
        },
        write: function(newValue) {
            var parsedValue = parseFloat(newValue);
            _actual(isNaN(parsedValue) ? newValue : parsedValue);
        }
    });

    return result;
};

ko.utils.clone = function (obj) {
    var target = new obj.constructor();
    for (var prop in obj) {
        var propVal = obj[prop];
        if (ko.isObservable(propVal)) {
            var val = propVal();
            if ($.type(val) == 'object') {
                target[prop] = ko.utils.clone(val);
                continue;
            }
            target[prop](val);
        }
    }
    return target;
};

var gz = {

    getNextId: (function()
                {
                    var seed = 1;
                    return function () {
                        return seed++;
                    };
                })(),

    deepClone: function(old) {
        var result = this.cloneObject(old);
        alert(result);
        return result;
    },
    //deepClone: function(old) { return jQuery.extend(true, {}, old); }

    cloneObject: function (obj) {
        if (obj === null || typeof obj !== 'object') {
            return obj;
        }

        var temp = obj.constructor(); // give temp the original obj's constructor
        for (var key in obj) {
            temp[key] = this.cloneObject(obj[key]);
        }

        return temp;
    }
}
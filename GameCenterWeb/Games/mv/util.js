var _2PI= Math.PI;

String.prototype.format = function () {
    var args = arguments;

    return this.replace(/\{(\d+)\}/g, function() {
        return args[arguments[1]];
    });
};

function getClone(donor) {
    var clone = $(donor).clone();
    clone.removeAttr('id');
    return clone;
}

function random(x) {
    return Math.floor(x * Math.random());
}

function getQueryParam(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function isString(v) {
    return typeof v == 'string' || v instanceof String;
}

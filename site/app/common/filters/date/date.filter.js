(function () {
    var app = angular.module('app');
    app.filter('ildate', function () {
        return function (input, format) {
            return moment(input).format(format);
        }
    });
})();
(function () {

    var app = angular.module('app');

    app.directive('emailExists', function (usersService, $q) {
        return {
            require: 'ngModel',
            scope: {
                emailExists: '<'
            },
            link: function (scope, elm, attrs, ctrl) {

                ctrl.$asyncValidators.emailExists = function (modelValue, viewValue) {

                    var deferred = $q.defer();
                    usersService.getUserIdByEmail(viewValue).then(function (res) {
                        if (scope.emailExists != res.data && res.data != 0) {
                            deferred.reject();
                        }
                        else {
                            deferred.resolve();
                        }
                    });
                    return deferred.promise;

                };
            }
        };
    });
})();
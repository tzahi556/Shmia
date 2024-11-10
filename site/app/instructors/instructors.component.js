(function () {

    var app = angular.module('app');

    app.component('instructors', {
        templateUrl: 'app/instructors/instructors.template.html',
        controller: InstructorsController,
        bindings: {
            users: '<'
        }
    });

    function InstructorsController() {
        var role = localStorage.getItem('currentRole');

       
        this.role = role;
    }

})();
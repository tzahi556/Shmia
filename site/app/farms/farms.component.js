(function () {
    var app = angular.module('app');

    app.component('farms', {
        templateUrl: 'app/farms/farms.template.html',
        controller: FarmsController,
        bindings: {
            farms: '<'
        }
    });

    function FarmsController(farmsService, authenticationService, $state) {
        this.enterManger = _enterManger.bind(this);

        function _enterManger(farmId) {

           

            farmsService.getFarmsMainUser(farmId).then(function (res) {
                localStorage.setItem('isSystemEnter', true);

             
                authenticationService.login({ userName: res.Email, password: res.Password }).then(function (res) {

                    location.href = './';
                },
                    function (res) {
                        alert(res.error_description);
                    });

            });


        }
    }

})();
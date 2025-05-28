(function () {

    var app = angular.module('app');

    app.component('farmName', {
        controller: FarmName,
        template: '{{$ctrl.farmName}}'
    });

    function FarmName(usersService, farmsService, $rootScope) {
      
        var DEFAULT_NAME = 'מערכת לניהול עובדים 101';
        var self = this;
        var authData = angular.fromJson(localStorage.getItem('authorizationData'));
        if (authData && authData.farmName) {
            self.farmName = authData.farmName;
        }
        else if (authData) {
            usersService.getUser().then(function (res) {
                localStorage.setItem('userLogin', res.FirstName + " " + res.LastName);
                localStorage.setItem('userLoginId', res.Id);
                farmsService.getFarm(res.Farm_Id).then(function (data) {
                   
                    self.farmName = data.Name ? data.Name : DEFAULT_NAME;
                    authData.farmName = self.farmName;
                   /* localStorage.setItem('FarmObj', data);*/
                    localStorage.setItem('FarmName', self.farmName);
                    localStorage.setItem('authorizationData', angular.toJson(authData));
                //    localStorage.setItem('FarmInstractorPolicy', data.Meta.IsInstractorPolicy);
                    localStorage.setItem('IsHiyuvInHashlama', data.IsHiyuvInHashlama);
                    localStorage.setItem('FarmStyle', data.Style);
                    localStorage.setItem('FarmId', res.Farm_Id);

                });
            });
        }
        else {
            self.farmName = DEFAULT_NAME;
        }


       // $rootScope.farmName = self.farmName;
        //alert(self.farmName);
    }

})();
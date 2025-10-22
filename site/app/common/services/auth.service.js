(function () {

    var app = angular.module('app');

    app.service('authenticationService', AuthenticationService);

    function AuthenticationService($http, $q, $location, sharedValues, usersService) {

        this.login = _login;
        this.logOut = _logOut;

        function _login(loginData) {


            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password + "&isfakeuser=" + loginData.isfakeuser;
            var deferred = $q.defer();
           
           
            $http.post(sharedValues.apiUrl + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).then(
              
              
                function (response) {
                  
                    var response = response.data;
                    localStorage.setItem('authorizationData', angular.toJson({ token: response.access_token, userName: loginData.userName }));

                   
                    usersService.getUser().then(function (res) {
                      
                        localStorage.setItem('currentRolesId', res.RolesId);
                        localStorage.setItem('HomePage', res.HomePage);
                        deferred.resolve(response);
                    })
                },
                function (response) {
                    _logOut();
                    deferred.reject(response.data);
                }

              

            );

            return deferred.promise;
        };

        function _logOut() {
            localStorage.removeItem('authorizationData');
            localStorage.removeItem('currentRolesId');
            //localStorage.removeItem('currentSubRole');
        };
    }
})();
(function () {

    var app = angular.module('app');

    app.service('authenticationService', AuthenticationService);

    function AuthenticationService($http, $q, $location, sharedValues, usersService) {

        this.login = _login;
        this.logOut = _logOut;

        function _login(loginData) {



            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;
            var deferred = $q.defer();

           
            $http.post(sharedValues.apiUrl + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).then(
              
              
                function (response) {
                  
                    var response = response.data;
                    localStorage.setItem('authorizationData', angular.toJson({ token: response.access_token, userName: loginData.userName }));

                   
                    usersService.getUser().then(function (res) {
                      
                        localStorage.setItem('currentRole', res.Role);
                        localStorage.setItem('currentSubRole', res.SubRole);
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
            localStorage.removeItem('currentRole');
            localStorage.removeItem('currentSubRole');
        };
    }
})();
(function () {
    
    var app = angular.module('app');

    app.component('login', {
        templateUrl: 'app/login/login.template.html',
        controller: LoginController,
        bindings: {
            returnUrl: '<',
           // loginimage: '<'
        }
    });

    function LoginController(authenticationService, $state) {
        this.login = _login;



        //if (!this.loginimage) {

        //    this.loginimage = localStorage.getItem('loginimage');


        //} else {

        //    localStorage.setItem('loginimage', this.loginimage);

        //}


        //alert(this.loginimage);


        //this.email = "tzahi556@gmail.com";
        //this.password = "123";
        function _login() {

           
            authenticationService.login({ userName: this.email, password: this.password }).then(function (res) {
               
                location.href = './';
            },
            function (res) {
                alert(res.error_description);
            });
        }
    }

})();
(function () {
    
    var app = angular.module('app');

    app.component('login', {
        templateUrl: 'app/login/login.template.html',
        controller: LoginController,
        bindings: {
            farm: '<',
        }
    });

    function LoginController(authenticationService, $state, sharedValues) {
        var self = this;
        this.login = _login;

       
        if (self.farm.Logo && self.farm.Logo.indexOf("http") == -1) {
            self.LogoTemp = "/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo;
            self.farm.Logo = sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo;
        } else if (!self.farm.Logo) {

            self.LogoTemp = "";
            self.farm.Logo = "../../favicon.png";
        }

     //   alert(localStorage.getItem('FarmId'));

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
                alertMessage(res.error_description,3);
            });
        }
    }

})();
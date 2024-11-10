(function () {
    var app = angular.module('app');

    app.directive('navigation', function () {
        return {
            templateUrl: 'app/common/components/navigation/navigation.template.html',
            controller: NavigationController,
            controllerAs: '$ctrl',
            replace: true,
        }
    });

    function NavigationController($scope, $rootScope) {
        this.init = _init.bind(this);

        $rootScope.$on('$stateChangeSuccess', this.init);

        function _init() {

         
            //$("#country").selectBoxIt({ /*autoWidth: false*/ });
            var role = localStorage.getItem('currentRole');
            this.role = role;

            this.subrole = localStorage.getItem('currentSubRole');

          
           
            this.farms = ['sysAdmin'].indexOf(role) != -1 ? true : false;
            //this.accounting = ['sysAdmin', 'farmAdmin', 'farmAdminHorse'].indexOf(role) != -1 ? true : false;
            //this.lessons = ['sysAdmin', 'farmAdmin', 'instructor', 'profAdmin', 'farmAdminHorse', "stableman", "worker", 'vetrinar', 'shoeing'].indexOf(role) != -1 ? true : false;
            this.users = ['sysAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            //this.instructors = (['sysAdmin', 'farmAdmin', 'farmAdminHorse', 'instructor', 'profAdmin', 'vetrinar', 'shoeing'].indexOf(role) != -1 && ["stableman", "worker"].indexOf(this.subrole) == -1 )? true : false;
            this.awsmangers = ['sysAdmin'].indexOf(role) != -1 ? true : false;
            this.workers = ['sysAdmin', 'farmAdmin', 'instructor'].indexOf(role) != -1 ? true : false;

            this.bindtodata = ['sysAdmin'].indexOf(role) != -1 ? true : false;

            this.docs = ['sysAdmin', 'farmAdmin', 'instructor'].indexOf(role) != -1 ? true : false;

            this.workersnew = ['sysAdmin', 'farmAdmin', 'instructor'].indexOf(role) != -1 ? true : false;

            this.reportarea = ['sysAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            this.logs = ['sysAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            //this.awsauto = ['sysAdmin', 'awsAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            //this.awsmangershistory = ['sysAdmin', 'awsAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            //this.awsmangershistoryphrase = ['sysAdmin', 'awsAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;


           // this.students = ['sysAdmin', 'awsAdmin', 'farmAdmin'].indexOf(role) != -1 ? true : false;
            //this.reports = ['sysAdmin', 'farmAdmin', 'farmAdminHorse', 'vetrinar', 'shoeing'].indexOf(role) != -1 ? true : false;
            //this.horses = ['sysAdmin', 'farmAdmin', 'profAdmin', 'stableman', "worker", 'assistant', 'farmAdminHorse', 'vetrinar', 'shoeing'].indexOf(role) != -1 || ["stableman", "worker"].indexOf(this.subrole) != -1 ? true : false;

            //this.farmmanager = ['sysAdmin', 'farmAdmin', 'farmAdminHorse'].indexOf(role) != -1 ? true : false;
            //this.files = role != null;

            var authData = localStorage.getItem('authorizationData');
            if (authData) {
                authData = angular.fromJson(authData);
                this.username = authData.userName;
            }
            else {
                this.username = null;
            }
        }


        //this.login = _login;

        //function _login() {

        //    alert(sharedValues.apiUrl);

        //    //window.location.href = 'http://localhost:61957/#/login/%D7%92%D7%99%D7%93%D7%99%D7%92%D7%93%D7%92%D7%97%20%D7%93%D7%92%D7%93%D7%92/';
            
        //}
    }

})();
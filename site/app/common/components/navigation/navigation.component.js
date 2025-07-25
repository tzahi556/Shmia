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

    function NavigationController($scope, $state, $rootScope) {
        this.init = _init.bind(this);

        $rootScope.$on('$stateChangeSuccess', this.init);

        function _init() {

         
            //$("#country").selectBoxIt({ /*autoWidth: false*/ });
            var role = localStorage.getItem('currentRolesId');
            this.role = role;

            //this.subrole = localStorage.getItem('currentSubRole');

          
           
            this.farms = ['0'].indexOf(role) != -1 ? true : false;
            //this.accounting = ['0', '2', '2Horse'].indexOf(role) != -1 ? true : false;
            //this.lessons = ['0', '2', '13','6', 'profAdmin', '2Horse', "stableman", "worker", 'vetrinar', 'shoeing'].indexOf(role) != -1 ? true : false;
            this.users = ['0', '2'].indexOf(role) != -1 ? true : false;
            //this.instructors = (['0', '2', '2Horse', '13','6', 'profAdmin', 'vetrinar', 'shoeing'].indexOf(role) != -1 && ["stableman", "worker"].indexOf(this.subrole) == -1 )? true : false;
            this.awsmangers = ['0', '2', '13', '6'].indexOf(role) != -1 ? true : false;

            this.workers = ['0', '2', '13','6'].indexOf(role) != -1 ? true : false;

            this.bindtodata = ['0'].indexOf(role) != -1 ? true : false;

            this.docs = ['0', '2', '13','6'].indexOf(role) != -1 ? true : false;

            this.workersnew = ['0', '2', '13','6'].indexOf(role) != -1 ? true : false;

            this.reportarea = ['0', '2'].indexOf(role) != -1 ? true : false;
            this.logs = ['0', '2'].indexOf(role) != -1 ? true : false;
            //this.awsauto = ['0', 'awsAdmin', '2'].indexOf(role) != -1 ? true : false;
            //this.awsmangershistory = ['0', 'awsAdmin', '2'].indexOf(role) != -1 ? true : false;
            //this.awsmangershistoryphrase = ['0', 'awsAdmin', '2'].indexOf(role) != -1 ? true : false;


           // this.students = ['0', 'awsAdmin', '2'].indexOf(role) != -1 ? true : false;
            //this.reports = ['0', '2', '2Horse', 'vetrinar', 'shoeing'].indexOf(role) != -1 ? true : false;
            //this.horses = ['0', '2', 'profAdmin', 'stableman', "worker", 'assistant', '2Horse', 'vetrinar', 'shoeing'].indexOf(role) != -1 || ["stableman", "worker"].indexOf(this.subrole) != -1 ? true : false;

            this.farmmanager = ['0', '2'].indexOf(role) != -1 ? true : false;
            this.campains = ['0', '2', '13', '6'].indexOf(role) != -1 ? true : false;

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


        this.login = _login;

        function _login() {



            localStorage.removeItem('authorizationData');
            localStorage.removeItem('currentRolesId');
            localStorage.removeItem('HomePage');

            $state.go('login');
            
            
        }
    }

})();
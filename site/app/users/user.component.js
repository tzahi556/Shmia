(function () {

    var app = angular.module('app');

    app.filter('sysAdminOnly', function () {
        return function (roles) {
            var isSysAdmin = localStorage.getItem('currentRole') == "sysAdmin";
            if (isSysAdmin) { return roles; }

            var returnRoles = [];
            for (var i in roles) {
                if (!roles[i].sysAdminOnly) {
                    returnRoles.push(roles[i]);
                }
            }
            return returnRoles;
        }
    });

    app.component('user', {
        templateUrl: 'app/users/user.template.html',
        controller: UserController,
        bindings: {
            user: '<'
           
        }
    });

    function UserController(usersService, $scope, $state, sharedValues) {
        this.scope = $scope;
        this.submit = _submit.bind(this);

        
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRole');
        this.areas = sharedValues.areas;

        //this.user.Areaid = this.user.Areaid;
       
        function _submit() {



            if (this.scope.userForm.$valid) {
               
                usersService.updateUser(this.user).then(function (user) {
                  
                    this.user = user;
                  
                  
                    alert('נשמר בהצלחה');
                }.bind(this));
            }
        }

        function _delete() {
            if (confirm('האם למחוק את המשתמש?')) {
                usersService.deleteUser(this.user.Id).then(function (res) {
                    $state.go('users');
                });
            }
        }
    }

})();
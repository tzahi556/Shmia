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
            user: '<',
           roles: '<'
        }
    });

    function UserController(usersService, $scope, $state, sharedValues) {
        this.scope = $scope;
        this.submit = _submit.bind(this);

        
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRolesId');
        this.areas = sharedValues.areas;
        this.init = _init.bind(this);
      

        this.init();
        function _init() {


           

            if (this.user.Id != 0) {

                this.user.Password = "12345678ssss9121234";
            } else {
                //this.Password = "";
               // debugger

                if (!this.user) this.user = {};

                this.user.Id = 0;
                this.user.Email = '';
                this.user.Password = '';
                this.user.FirstName = '';
                this.user.LastName = '';
                this.user.PhoneNumber = '';
                this.user.RolesId = null;
                this.user.StatusId = 1;
                   
                
               
            }
        }
        
        function _submit() {



            if (this.scope.userForm.$valid) {

                
                if (this.user.Id == 0 && !this.user.Password) {
                    alertMessage("חובה לשים סיסמה על משתמש/ת חדש/ה!", 3);
                    return;

                }


                if (this.user.Id != 0) {

                    this.user.Password = null;
                }

                usersService.updateUser(this.user).then(function (user) {
                  
                    this.user = user;
                  
                  
                    alertMessage('הנתונים נשמרו בהצלחה!');
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

        this.OpenDialog = function () {
            $("#dialogContainer").dialog({
                modal: true,
                height: 'auto',
                width: 'auto',
                resizable: true
            });

            $("#dialogContainer").dialog("option", "position", { my: "center", at: "center", of: window });
        };


        this.saveNewPassword = function () {

            this.user.Password = this.newpassword;
            usersService.updateUser(this.user).then(function (user) {
              
                this.user = user;
                this.user.Password = this.newpassword;
                alertMessage('סיסמה חדשה עודכנה בהצלחה!');
            }.bind(this));
        };



    }

})();
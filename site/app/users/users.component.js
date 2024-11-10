(function () {

    var app = angular.module('app');

    app.filter('filterRoles', function (usersService) {
        return function (users) {
            var returnUsers = [];
            for (var i in users) {

                // users[i].Role = this.sharedValues.roles.filter(x => x.id == users[i].Role)[0].name;//(users[i].Role);
                if (showInUsers(users[i].Role)) {


                    returnUsers.push(users[i]);
                }
            }
            return returnUsers;
        }





        function showInUsers(role) {
            var roles = usersService.roles;
            for (var i in roles) {
                if (roles[i].id == role && roles[i].showInUsers) {
                    return true;
                }
            }
            return false;
        }
    });

    app.component('users', {
        templateUrl: 'app/users/users.template.html',
        controller: UsersController,
        bindings: {
            users: '<'

        }
    });

    function UsersController(usersService, sharedValues) {

        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);
        this.getHebArea = _getHebArea.bind(this);
        function _getHebRole(id) {


            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }

        function _getHebArea(id, num) {

            if (!id) return "";
            var res = this.sharedValues.areas.filter(x => x.id == id);
            if (res.length > 0) {

                res = this.sharedValues.areas.filter(x => x.id == id)[0].name;

                if (num) res = "," + res;
                return res;//(users[i].Role);
            }
            else
                return "";
        }





    }

})();
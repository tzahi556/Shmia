(function () {

    var app = angular.module('app');


    app.component('logs', {
        templateUrl: 'app/report/logs.template.html?v=2',
        controller: LogsController,
        bindings: {
            logs: '<',
            users: '<'

        }
    });

    function LogsController(usersService, sharedValues, $state) {


        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);

        this.delete = _delete.bind(this);

        this.search = _search.bind(this);
        this.uploadsUri = sharedValues.apiUrl + '/uploads/';

        this.StartDate = moment().add(-5, 'days').toDate();
        this.EndDate = moment().add(1, 'days').toDate();

        function _getHebRole(id) {


            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }

        function _delete(workerid) {
            if (confirm('האם למחוק את העובד?')) {

                var ctrl = this;
                usersService.deleteWorker(workerid, true).then(function (res) {


                    ctrl.workers = res;
                    // $state.go('workers');
                    //ctrl.user.Deleted = true;
                    //$state.go('students');
                });
            }
        }

        function _search() {

            var StartDate = moment(this.StartDate.toString()).format("YYYY-MM-DD HH:mm");
            var EndDate = moment(this.EndDate.toString()).format("YYYY-MM-DD HH:mm");
            var UserId = this.UserId;
            var ctrl = this;
            usersService.getLogsData(UserId,StartDate, EndDate).then(function (res) {


                ctrl.logs = res;

            });

        }




    }

})();
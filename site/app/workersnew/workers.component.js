(function () {

    var app = angular.module('app');

  
    app.component('workersnew', {
        templateUrl: 'app/workersnew/workers.template.html',
        controller: WorkersnewController,
        bindings: {
            workersnew: '<'
            
        }
    });

    function WorkersnewController(usersService, sharedValues, $state) {

   
        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);

        this.delete = _delete.bind(this);
        this.uploadsUri = sharedValues.apiUrl + '/uploads/';
        function _getHebRole(id) {

        
            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }


        function _delete(workerid) {
            if (confirm('האם למחוק את העובד?')) {
              
                var ctrl = this;
                usersService.deleteWorker(workerid,false).then(function (res) {

                   
                    ctrl.workersnew = res;
                   // $state.go('workers');
                    //ctrl.user.Deleted = true;
                    //$state.go('students');
                });
            }
        }
    }

})();
(function () {

    var app = angular.module('app');

  
    app.component('workers', {
        templateUrl: 'app/workers/workers.template.html?v=2',
        controller: WorkersController,
        bindings: {
            workers: '<'
            
        }
    });

    function WorkersController(usersService, sharedValues, $state) {

   
        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);
        this.checkAll = _checkAll.bind(this);
        this.deleteAll = _deleteAll.bind(this);
        

        this.delete = _delete.bind(this);
        this.uploadsUri = sharedValues.apiUrl + '/uploads/';


        function _checkAll() {

         
            this.workers.forEach(x => x.IsSelected = this.checkAllc);
        }




        function _getHebRole(id) {

        
            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }

        function _deleteAll() {

            if (confirm('האם למחוק את כל העובדים המסומנים?')) {


                var selected = this.workers.filter(x => x.IsSelected);

                for (var i in selected) {
                    var ctrl = this;
                    usersService.deleteWorker(selected[i].Id, true).then(function (res) {


                         ctrl.workers = res;
                       
                    });

                }


            }

        }
        function _delete(workerid) {
            if (confirm('האם למחוק את העובד?')) {
              
                var ctrl = this;
                usersService.deleteWorker(workerid,true).then(function (res) {

                   
                    ctrl.workers = res;
                   // $state.go('workers');
                    //ctrl.user.Deleted = true;
                    //$state.go('students');
                });
            }
        }
    }

})();
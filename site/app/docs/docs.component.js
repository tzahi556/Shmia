(function () {

    var app = angular.module('app');



    app.component('docs', {
        templateUrl: 'app/docs/docs.template.html',
        controller: DocsController,
        bindings: {

        }
    });

    function DocsController(usersService, $scope, $state, sharedValues, filesService, $window) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        //this.submit = _submit.bind(this);
        this.roles = usersService.roles;

        this.role = localStorage.getItem('currentRole');

        this.uploadFile = _uploadFile.bind(this);
        this.removeFile = _removeFile.bind(this);
        this.sendForms = _sendForms.bind(this);

        this.sendfiles = [];
        this.files = [];

        function _uploadFile(changeEvent) {

            var uploadFiles = changeEvent.target.files;

            //  this.sendfiles = uploadFiles || [];

            for (var i = 0; i < uploadFiles.length; i++) {
                var Obj = uploadFiles[i];
                this.sendfiles.push(Obj);

            }

            //for (var i in uploadFiles) {

            //    this.files.push(uploadFiles[i]);

            //}

            //  debugger

            //this.files = this.files || [];
            //var allfiles = file.split(",") || [];



            //for (var i in allfiles) {
            //    var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
            //    this.files.push(Obj);
            //    this.tazfiles.push(Obj);
            //}

        }

        function _removeFile(file, type) {



            for (var i in this.sendfiles) {
                if (this.sendfiles[i].name == file) {

                    this.sendfiles.splice(i, 1);
                    break;
                }
            }





        }

        function _sendForms() {
           

            filesService.uploadAfterPost(this.sendfiles, "sendmail", "0", this.Comment).then(function (data) {

                if (data == "false") {
                    alertMessage("תקלה בשליחת מייל , אנא נסי במעוד מאוחר יותר!");
                    return;
                }
                this.Comment = "";
                this.sendfiles = [];


                alertMessage("מייל נשלח למשרד!");
               
            });

        }

        

    }



})();
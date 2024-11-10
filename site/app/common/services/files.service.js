(function () {

    var app = angular.module('app');

    app.service('filesService', FilesService);

    function FilesService($q, $http, sharedValues) {
        this.upload = _upload.bind(this);
        this.uploadAfterPost = _uploadAfterPost.bind(this);
        this.delete = _delete.bind(this);

        function _upload(file,folder,workerid) {
         
           
            var deferred = $q.defer();
            var fd = new FormData();
           
            for (var i = 0; i < file.length; i++) {
                fd.append('file', file[i]);
            }
           
            $http.post(sharedValues.apiUrl + 'files/upload/' + folder + '/' + workerid, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).then(
            function (data) {
                deferred.resolve(data.data);
            },
            function () {
                alert('לא ניתן לעלות תמונה, אין גישה לרשת');
                deferred.reject();
            });
            return deferred.promise;
        }

        function _uploadAfterPost(file, folder, workerid,text) {


            var deferred = $q.defer();
            var fd = new FormData();

            for (var i = 0; i < file.length; i++) {
                fd.append('file', file[i]);
            }

           

            $http.post(sharedValues.apiUrl + 'files/uploadformail/' + folder + '/' + workerid + '/' + text, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).then(
                function (data) {
                    deferred.resolve(data.data);
                },
                function () {
                    alert('לא ניתן לעלות תמונה, אין גישה לרשת');
                    deferred.reject();
                });
            return deferred.promise;
        }

        function _delete(filename) {
            return $http.get(sharedValues.apiUrl + 'files/delete?filename=' + filename);
        }
    }

    app.directive("filename", ['filesService', function (filesService) {
        return {
            scope: {
                filename: "=",
                filenameCallback: "=",
                workerid: "=",
                folder: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {

                    debugger

                    if (changeEvent.target.files.length > 0) {
                        filesService.upload(changeEvent.target.files, scope.folder, scope.workerid).then(function (data) {
                            
                            //scope.filename = data;
                            if (scope.filenameCallback) {
                                scope.filenameCallback(data);
                            }
                        });
                    }
                });
            },
        }
    }]);

    app.directive("filenamenotupload", ['filesService', function (filesService) {
        return {
            scope: {
                filenamenotupload: "=",
                filenameCallback: "=",
                workerid: "=",
                folder: "="
               
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {

                    if (changeEvent.target.files.length > 0) {
                     
                            //if (scope.filenameCallback) {
                            //    scope.filenameCallback(changeEvent);
                            //}
                     
                        filesService.uploadAfterPost(changeEvent.target.files, scope.folder, scope.workerid,"ss").then(function (data) {

                            //scope.filename = data;
                            if (scope.filenameCallback) {
                                scope.filenameCallback(changeEvent);
                            }
                        });
                    }
                });
            },
        }
    }]);

    app.directive("imagepicker", function (sharedValues, filesService) {
        return {
            scope: {
                imageLabel: "@",
                model: "=",
            },
            link: function (scope, element, attributes) {
                scope.delete = _delete;
                function _delete() {
                    filesService.delete(scope.model);
                    scope.model = null;
                }
            },
            template:
                '<div class="image-picker">\
                <label ng-bind="imageLabel"></label>\
                <div class="form-group">\
                <input type="file" multiple class="form-control" filename="model" />\
                </div>\
                <img style="width:100%;height:auto;" ng-src="' + sharedValues.apiUrl + 'uploads/{{model}}" ng-if="model" />\
                <div><button ng-click="delete()" type="button" style="width:100%;margin-top:5px;" ng-show="model" class="btn btn-danger">מחיקה</button></div>\
                </div>',

            replace: true,
        }
    });

})();
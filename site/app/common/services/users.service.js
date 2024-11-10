(function () {

    var app = angular.module('app');

    app.service('usersService', UsersService);

    function UsersService(sharedValues, $http, $q) {
        this.getUsers = _getUsers;
        this.getUser = _getUser;
        this.updateUser = _updateUser;
      
        this.importPortfolios = _importPortfolios;
        this.deleteUser = _deleteUser;
        this.getUserIdByEmail = _getUserIdByEmail;
        this.roles = _roles();
       
        this.getPortfolios = _getPortfolios;
        this.updatePortfolios = _updatePortfolios;
        this.getAllAsins = _getAllAsins;
        this.updatePortfoliosAsins = _updatePortfoliosAsins;

        this.getAllPortfliositemsForWords = _getAllPortfliositemsForWords;
        this.updatePortfoliosItemsNegative = _updatePortfoliosItemsNegative;

        this.getPortfoliosItems = _getPortfoliosItems;
        this.getPortfoliosWords = _getPortfoliosWords;

        this.setWords = _setWords;
        this.GetHistoryData = _GetHistoryData;
        this.SetHistoryData = _SetHistoryData;

        this.MWSService = _MWSService;
        this.setStatusForTerm = _setStatusForTerm;
        this.getWorkers = _getWorkers;
        this.getWorker = _getWorker;
        this.getCitiesList = _getCitiesList;
        this.deleteWorker = _deleteWorker;
        this.updateWorker = _updateWorker;
        this.getFiles = _getFiles;
        this.getWorkerChilds = _getWorkerChilds;
        this.bindData = _bindData;
        this.getMasterTable = _getMasterTable;

        this.getReportData = _getReportData;
        this.downloadAllManagerFiles = _downloadAllManagerFiles;
        
        this.importWorkers = _importWorkers;

        this.getLogsData = _getLogsData;


        function _getLogsData(userid, start, end) {
          
            var deferred = $q.defer();
            $http.get(sharedValues.apiUrl + 'users/getLogsData/', { params: { userid: userid, start: start, end: end } }).then(function (res) {

                deferred.resolve(res.data);
            });


            return deferred.promise;
        }


        function _importWorkers(portfolioItems, counter) {
           
            var deferred = $q.defer();
            $http.post(sharedValues.apiUrl + 'users/importWorkers/' + counter, portfolioItems).then(function () {
                deferred.resolve();
            });
            return deferred.promise;
        }




        function _getFiles(workerid) {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getFiles/' + workerid).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        

        function _downloadAllManagerFiles(Id, Shnatmas) {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/downloadAllManagerFiles/' + Id + '/' + Shnatmas).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }



        function _getReportData(type) {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getReportData/' + type).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        



        function _getMasterTable(type) {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getMasterTable/' + type).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        //************************** Worker ****************




        function _getWorkers(isnew) {
            
            ///if (isnew == -1) isnew = true;
            var deferred = $q.defer();
         
            $http.get(sharedValues.apiUrl + 'users/getWorkers/' + isnew).then(function (res) {
               
                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        function _getWorker(id) {

          
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getWorker/' + id).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        function _deleteWorker(id,isnew) {
            var deferred = $q.defer();


            $http.get(sharedValues.apiUrl + 'users/deleteWorker/' + id + '/' + isnew).then(function (res) {
             
                var res = res.data;
                deferred.resolve(res);
            });
            return deferred.promise;
        }

        function _updateWorker(worker, files, childs,type) {

            var dataobj = [worker, files, childs];
            var deferred = $q.defer();

           

            $http.post(sharedValues.apiUrl + 'users/updateWorker/' + type ,  angular.toJson(dataobj)).then(function (res) {

                var worker = res.data;
                deferred.resolve(worker);
            });
            return deferred.promise;
        }

        function _getWorkerChilds(workerid) {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getWorkerChilds/' + workerid ).then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

         //************************** End Worker ****************
         //************************** Master Data ****************
        function _getCitiesList() {


            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getCitiesList/').then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

         //************************** End Worker ****************

        function _setStatusForTerm(portfolioitemId, changeType) {
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/SetStatusForTerm/' + portfolioitemId + '/' + changeType).then(function () {

                //var data = res.data;
                deferred.resolve("true");

            });
            return deferred.promise;
        }

        function _MWSService() {
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/MWSService/').then(function () {

                //var data = res.data;
                deferred.resolve("true");

            });
            return deferred.promise;
        }


        
        function _GetHistoryData(type, Id) {
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/GetHistoryData/' + type + '/' + Id).then(function (res) {

                var data = res.data;
                deferred.resolve(data);

            });
            return deferred.promise;
        }






        
        function _SetHistoryData(counter, portfolioAsin) {
            var deferred = $q.defer();

            $http.post(sharedValues.apiUrl + 'users/SetHistoryData/', portfolioAsin).then(function (res) {

                var data = res.data;
                deferred.resolve(data);

            });
            return deferred.promise;
        }

        function _setWords(portfolioid, newHardvalue, type, actionto) {

            newHardvalue = [newHardvalue];

           // if (!count) count = 0;
            var deferred = $q.defer();

            $http.post(sharedValues.apiUrl + 'users/setWords/' + portfolioid + '/' + type + '/' + actionto, newHardvalue).then(function (res) {

                var Portfoliowords = res.data;
                deferred.resolve(Portfoliowords);
            });
            return deferred.promise;
        }
        

        function _updatePortfoliosItemsNegative(portfolioid, portfolioitems) {

         
            var deferred = $q.defer();

            $http.post(sharedValues.apiUrl + 'users/updatePortfoliosItemsNegative/' + portfolioid, portfolioitems).then(function (res) {

                var PortfolioItems = res.data;
                deferred.resolve(PortfolioItems);
            });
            return deferred.promise;
        }


        function _getPortfoliosItems() {
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getPortfoliosItems/').then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }



        //function _GetHistoryData() {
        //    var deferred = $q.defer();

        //    $http.get(sharedValues.apiUrl + 'users/getPortfoliosItems/').then(function (res) {

        //        var res = res.data;
        //        deferred.resolve(res);

        //    });
        //    return deferred.promise;
        //}
        





        function _getPortfoliosWords() {
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/getPortfoliosWords/').then(function (res) {

                var res = res.data;
                deferred.resolve(res);

            });
            return deferred.promise;
        }

        function _getAllPortfliositemsForWords(id, count, positiveWords) {
            var deferred = $q.defer();
           
            $http.post(sharedValues.apiUrl + 'users/getAllPortfliositemsForWords/' + id + '/' + count, positiveWords).then(function (res) {

                var portfoliowords = res.data;
                deferred.resolve(portfoliowords);

            });
            return deferred.promise;
        }

        function _getPortfolios(llx, lly, urx, ury, text, font, space, id,pagenumber) {

            if (!text) text = "1";
            if (!pagenumber) pagenumber = "1";
            if (!font) font = 12;
            if (!space) space = 1;
            var deferred = $q.defer();
           
            $http.get(sharedValues.apiUrl + 'users/getPortfolios/' + llx + '/' + lly + '/' + urx + '/' + ury + '/' + text + '/' + font + '/' + space + '/' + id + '/' + pagenumber).then(function (res) {
             
                var portfolios = res.data;
                deferred.resolve(portfolios);

            });
            return deferred.promise;
        }

        function _bindData(id,comment,pagenumber,value) {
            if (!comment) comment = 'null';
            if (!value) value = 'null';
            
            var deferred = $q.defer();

            $http.get(sharedValues.apiUrl + 'users/bindData/' + id + '/' + comment + '/' + pagenumber + '/' + value).then(function (res) {

                var portfolios = res.data;
                deferred.resolve(portfolios);

            });
            return deferred.promise;
        }

        function _getUsers(role, includeDeleted) {
            var deferred = $q.defer();
          
            $http.get(sharedValues.apiUrl + 'users/getusers' + (role ? '/' + role : '') + (includeDeleted ? '/' + includeDeleted : '')).then(function (res) {
              
                var users = res.data;
                deferred.resolve(users);
              
            });
            return deferred.promise;
        }

        function _getUser(id, isForCartis) {
          
            if (!isForCartis) isForCartis = false;

           
            var deferred = $q.defer();
            if (id == 0) {
                $http.get(sharedValues.apiUrl + 'users/newuser/').then(function (res) {
                    res.data.Meta = {};
                    deferred.resolve(res.data);
                });
            }
            else {

                if (isForCartis) {
                    $http.get(sharedValues.apiUrl + 'users/getsetUserEnter/' + isForCartis + '/' + (id || '')).then(function (res) {
                        var user = res.data;
                        deferred.resolve(user);
                    });


                } else {
                    $http.get(sharedValues.apiUrl + 'users/getUser/' + (id || '')).then(function (res) {
                        var user = res.data;
                        deferred.resolve(user);
                    });


                }


              
            }
            return deferred.promise;
        }

        function _getPaymentsByUserId(id) {
            var deferred = $q.defer();
            $http.get(sharedValues.apiUrl + 'users/getpaymentsbyuserid/' + (id || '')).then(function (res) {
              
                var user = res.data;
                
                deferred.resolve(user);
            });
            return deferred.promise;
        }

        function _updateUser(user) {
           
            var deferred = $q.defer();
         //   user.Meta = angular.toJson(user.Meta);
           
            $http.post(sharedValues.apiUrl + 'users/updateuser', user).then(function (res) {
             
                var user = res.data;
              //  user.Meta = angular.fromJson(user.Meta);
                deferred.resolve(user);
            });
            return deferred.promise;
        }

        function _importPortfolios(portfolioItems,counter) {
            var deferred = $q.defer();
            $http.post(sharedValues.apiUrl + 'users/importPortfolios/' + counter, portfolioItems).then(function () {
                deferred.resolve();
            });
            return deferred.promise;
        }


        function _updatePortfolios(Portfolios) {

            var deferred = $q.defer();

            $http.post(sharedValues.apiUrl + 'users/updatePortfolios', Portfolios).then(function (res) {

                var Portfolios = res.data;
                deferred.resolve(Portfolios);
            });
            return deferred.promise;
        }

        function _updatePortfoliosAsins(count, PortfolioAsins, portfolioid) {
           
            if (!count) count= 0;
            var deferred = $q.defer();

            $http.post(sharedValues.apiUrl + 'users/updatePortfoliosAsins/' + count + '/' + portfolioid, PortfolioAsins).then(function (res) {

                var PortfolioItems = res.data;
                deferred.resolve(PortfolioItems);
            });
            return deferred.promise;
        }

        function _deleteUser(id) {
            var deferred = $q.defer();
           

            $http.get(sharedValues.apiUrl + 'users/deleteUser/' + id).then(function (res) {
                deferred.resolve();
            });
            return deferred.promise;
        }

        function _getAllAsins(id) {
           
            var deferred = $q.defer();
            $http.get(sharedValues.apiUrl + 'users/getAllAsins/' + id).then(function (res) {
               
                var user = res.data;

                deferred.resolve(user);
            });
            return deferred.promise;
        }

        function _getUserIdByEmail(email) {
            return $http.get(sharedValues.apiUrl + 'users/getUserIdByEmail/' + email + '/');
        }

        function _roles() {
            return sharedValues.roles;
        }


    }

})();
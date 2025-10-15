(function () {

    var app = angular.module('app');

   // https://test.dgtracking.co.il/#/worker/1880/

    app.component('workerall', {
        templateUrl: 'app/workersall/workerall.template.html?v=3',
        controller: WorkerallController,
        bindings: {
            worker: '<',
            //files: '<',
            //childs: '<',
            cities: '<',
            departments: '<'
            //banks: '<',
            //banksbrunchs: '<',
            //users: '<',
            //screendata: '<',
            //farm: '<',
            //campainsstatustype: '<'

        }
    });

    function WorkerallController(usersService, farmsService, $scope, $state, sharedValues, filesService, $window, $timeout) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        this.submit = _submit.bind(this);
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        // this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRolesId');
        this.farmStyle = localStorage.getItem('FarmStyle');
        this.farmid = localStorage.getItem('FarmId');


        



      
     
        this.init = _init.bind(this);
       

     
        this.saveWorker = _saveWorker.bind(this);
       
        this.changeDateFormat = _changeDateFormat.bind(this);

        

        this.uploadsUri = sharedValues.apiUrl + '/uploads/'
      
        this.foldertaz = "taz";

        self = this;
        this.fileparud = "";

        // this.childs = [];

        this.ImageSignuture;
       
        this.init();


      
        $scope.getGroupsDetails = function (groupId) {

            return self.screendata.filter(x => x.f2g != null && x.f2g.FieldsGroupsId == groupId);
            //return self.screendata.filter(x => x.f2g.FieldsGroupsId == groupId);


            //return item.WorkerTableField === '1' || item.WorkerTableField === '2';
        };













        function uniqueBy(arr, prop, prop2, tempRes) {
            return arr.reduce((a, d) => {
                if (!a.includes(d[prop][prop2]))
                {
                    a.push(d[prop][prop2]);
                    tempRes.push(d[prop]);
                }
                return a;
            }, []);
        }

        function _init() {
           this.worker.w.StatusId = this.worker.w.StatusId.toString();
           // alert(this.worker.w.StatusId);
            //if (!this.worker.w101) {
            //    this.worker.w101 = {ShnatMas:"2025"};
            //}

            function setDateForArray(obj) {

                Object.keys(obj).forEach(function (key, index) {

                    if (key.indexOf("Date") != -1 && obj[key] && key != "DateRigster") {
                        
                        obj[key] = new Date(moment(obj[key]).format("YYYY-MM-DD"));// .startOf('day').toDate();

                       
                    }


                });

            }


           
         
           


            if (!this.worker.w.FarmId)
                 this.worker.farmid = this.farmid;

       
            
       

               
            if (this.worker.w) setDateForArray(this.worker.w);
     
          


        
            setTimeout(function () {

                autocomplete(document.getElementById("txtCity"), $scope.$ctrl.cities);
                
            }, 1000);
           
        }

        


      


        function ValidateID(str) {

            // DEFINE RETURN VALUES
            var R_ELEGAL_INPUT = -1;
            var R_NOT_VALID = -2;
            var R_VALID = 1;

            //INPUT VALIDATION

            // Just in case -> convert to string
            var IDnum = str;

            // Validate correct input
            if ((IDnum.length > 9) || (IDnum.length < 5))
                return R_ELEGAL_INPUT;
            if (isNaN(IDnum))
                return R_ELEGAL_INPUT;

            // The number is too short - add leading 0000
            if (IDnum.length < 9) {
                while (IDnum.length < 9) {
                    IDnum = '0' + IDnum;
                }
            }

            // CHECK THE ID NUMBER
            var mone = 0, incNum;
            for (var i = 0; i < 9; i++) {
                incNum = Number(IDnum.charAt(i));
                incNum *= (i % 2) + 1;
                if (incNum > 9)
                    incNum -= 9;
                mone += incNum;
            }
            if (mone % 10 == 0)
                return R_VALID;
            else
                return R_NOT_VALID;
        }

       

        function _changeDateFormat(dateVal) {



            //var dd = dateVal.substring(0, 2);
            //var mm = dateVal.substring(3, 5);
            //var yy = dateVal.substring(6, 11);



            //return yy + "-" + mm + "-" + dd;


           
            var d = new Date(dateVal),
                    month = '' + (d.getMonth() + 1),
                    day = '' + d.getDate(),
                    year = d.getFullYear();

                if (month.length < 2)
                    month = '0' + month;
                if (day.length < 2)
                   day = '0' + day;

            if (year < 1900 || year > 2100) return false;


                return [year, month, day].join('-');
           



        }

        function SaveDynamicData(worker) {

            var fields2GroupsWorkerDataList = [];

          

            for (var i = 0; i < self.screendata.length; i++) {
                if (!self.screendata[i].f2g) continue;

                if (self.screendata[i].f2gwd) {

                   

                    if (self.screendata[i].f2g.FieldsDataTypesId == 3 && self.screendata[i].f2gwd.Value)
                        self.screendata[i].f2gwd.Value = self.changeDateFormat(self.screendata[i].f2gwd.Value);

                    self.screendata[i].f2gwd.WorkersId = worker.w.Id;


                    fields2GroupsWorkerDataList.push(self.screendata[i].f2gwd);
                }
            }


            
            farmsService.getSetWorkerAndCompanyData(2, self.worker.w.Id, fields2GroupsWorkerDataList).then(function (screendata) {

               self.screendata = screendata;

               self.init();

               //debugger
                //alertMessage('הנתונים נשssssמרו בהצלחה!');

            }.bind(self));


        }

        function _saveWorker(type) {

            var thisCtrl = this;

            var IsInvalid = false;

            function setRightDate(obj) {

                Object.keys(obj).forEach(function (key, index) {

                    if (key.indexOf("Date") != -1 && obj[key] && key != "DateRigster") {

                        var strDate = thisCtrl.changeDateFormat(obj[key]);
                        if (strDate)
                            obj[key] = strDate;
                        else {
                            IsInvalid = true;
                            return;
                        }
                    }

                });


            }


           
           
            try {

                
              
              

                var obj = this.worker.w; //angular.copy(this.worker.w);

                setRightDate(obj);

                //var obj101 = this.worker.w101;//angular.copy(this.worker.w101);

                //setRightDate(obj101);

                if (IsInvalid) {


                    alertMessage("נא להכניס תאריך נכון בשדות תאריך!", 3);
                    return;
                }

                if (type == 1) {

                  
                

                    usersService.updateWorkerAll(obj, type).then(function (worker) {
                        //  this.worker = worker;
                       // SaveDynamicData(worker);
                        alertMessage('הנתונים נשמרו בהצלחה!');
                       
                    }.bind(this));

                }

                if (type == 2) {
                    if (this.scope.workerForm.$valid) {


                        if (this.tazfiles.length == 0) {

                            alertMessage('חובה לצרף צילום תעודת זהות!');
                            return;
                        }


                      
                        var Signature = $scope.accept();
                        if (!Signature.isEmpty) {
                            obj101["ImgData"] = Signature.dataUrl;
                        } else {
                            obj101["ImgData"] = "";

                        }

                       
                        SaveDynamicData(this.worker);
                        usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {


                            var m = this.campainsstatustype.filter(x => x.Id == worker.w101.StatusId);
                            if (m.length > 0) {

                                if (worker.w101.StatusId == 10) {

                                    alertMessage(m[0].Name);
                                } else {

                                    alertMessage(m[0].Name, 3);

                                }
                            }
                            
                           

                        }.bind(this));
                    }

                    else {
                        alertMessage("יש למלא את כל השדות המסומנים באדום , אלו שדות חובה",3);

                    }


                }

                if (type == 3) {
                   // $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ PDF  <br/>אנא המתנ/י...</div></h5>' });

                    var Signature = $scope.accept();
                    if (!Signature.isEmpty) {
                        obj101["ImgData"] = Signature.dataUrl;
                    } else {
                        obj101["ImgData"] = "";

                    }


                    SaveDynamicData(this.worker);
                    usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {
                        
                       //$.unblockUI();
                        $window.open(this.uploadsUri + "Workers/" + this.worker.w.Id + "/-1/"+  "/AllPdfTemp.pdf", '_blank');
                        //$window.open(this.uploadsUri + "Workers/2/AllPdfTemp.pdf", '_blank');
                    }.bind(this));

                }
            } catch (err) {
                alertMessage(err.message, 3);
              

            }

        }

        function _submit() {

        }

        function _delete() {

            var dd = this.worker;
            if (confirm('האם למחוק את המשתמש?')) {
                usersService.deleteUser(this.user.Id).then(function (res) {
                    $state.go('workers');
                });
            }
        }


    }



})();
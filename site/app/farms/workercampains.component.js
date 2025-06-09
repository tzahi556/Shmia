(function () {

    var app = angular.module('app');

    // https://test.dgtracking.co.il/#/worker/1880/

    app.component('workercampains', {
        templateUrl: 'app/farms/workercampains.template.html?v=3',
        controller: WorkercampainsController,
        bindings: {
            worker: '<',
            campain: '<',
            //childs: '<',
            //cities: '<',
            //banks: '<',
            //banksbrunchs: '<',
            //users: '<',
            screendata: '<',
            //farm: '<'

        }
    });

    function WorkercampainsController(usersService, farmsService, $scope, $stateParams, sharedValues, filesService, $window, $timeout) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        //this.submit = _submit.bind(this);
        this.roles = usersService.roles;

        this.role = localStorage.getItem('currentRole');
        this.farmStyle = localStorage.getItem('FarmStyle');
        this.farmid = localStorage.getItem('FarmId');
        this.clearSignuture = _clearSignuture.bind(this);
        this.init = _init.bind(this);
        this.saveWorker = _saveWorker.bind(this);
        this.changeDateFormat = _changeDateFormat.bind(this);
        this.ImageSignuture;
        this.uploadsUri = sharedValues.apiUrl + '/uploads/'

        this.init();

        self = this;

        $scope.getGroupsDetails = function (groupId) {

            return self.screendata.filter(x => x.f2g != null && x.f2g.FieldsGroupsId == groupId);

        };

        function uniqueBy(arr, prop, prop2, tempRes) {
            return arr.reduce((a, d) => {
                if (!a.includes(d[prop][prop2])) {
                    a.push(d[prop][prop2]);
                    tempRes.push(d[prop]);
                }
                return a;
            }, []);
        }

        function _init() {

            //alert($stateParams.id);

            function setDateForArray(obj) {

                Object.keys(obj).forEach(function (key, index) {

                    if (key.indexOf("Date") != -1 && obj[key] && key != "DateRigster") {

                        obj[key] = new Date(moment(obj[key]).format("YYYY-MM-DD"));// .startOf('day').toDate();


                    }


                });

            }

            this.groupsonly = [];

            uniqueBy(this.screendata, "fg", "Id", this.groupsonly);

            for (var i = 0; i < this.screendata.length; i++) {

                if (!this.screendata[i].f2g) continue;

                if (this.screendata[i].f2g.FieldsDataTypesId == 4 && this.screendata[i].f2gwd.Value) {
                    this.screendata[i].f2gwd.Value = eval(this.screendata[i].f2gwd.Value);
                }


                if (this.screendata[i].f2g.FieldsDataTypesId == 3 && this.screendata[i].f2gwd.Value) {

                    this.screendata[i].f2gwd.Value = new Date(moment(this.screendata[i].f2gwd.Value).format("YYYY-MM-DD"));
                }
            }


            //alert(this.worker.IsHaveSignature);
            /* alert(this.campain.MustSign);*/
        }

        function _clearSignuture() {

            $scope.clear();

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

        function SaveDynamicData(type, worker) {

            var fields2GroupsWorkerDataList = [];



            for (var i = 0; i < self.screendata.length; i++) {
                if (!self.screendata[i].f2g) continue;

                if (self.screendata[i].f2gwd) {


                    if (self.screendata[i].f2g.FieldsDataTypesId == 3 && self.screendata[i].f2gwd.Value)
                        self.screendata[i].f2gwd.Value = self.changeDateFormat(self.screendata[i].f2gwd.Value);

                    self.screendata[i].f2gwd.WorkersId = worker.Id;


                    fields2GroupsWorkerDataList.push(self.screendata[i].f2gwd);
                }
            }



            var ObjPost = { "Fields2GroupsWorkerData": fields2GroupsWorkerDataList, "InnerType": type, "Workers": worker };

            //ObjPost.Fields2GroupsWorkerData = fields2GroupsWorkerDataList;
            //ObjPost.InnerType = type;
            //if (worker["ImgData"]) ObjPost.ImgData = { "ImgData": worker["ImgData"] };


           
            farmsService.getSetWorkerAndCompanyData(22, self.worker.Id, ObjPost, self.campain.Id).then(function (screendata) {

                //if (worker["ImgData"]) {

                //    var ImgData = { "ImgData": worker["ImgData"] };

                //    farmsService.getSetWorkerAndCompanyData(22, self.worker.Id, ImgData, self.campain.Id).then(function (screendata2) {

                //    }.bind(self));
                //}

                // self.screendata = screendata;

                //self.init();
                if (type == 1)
                    alertMessage('הנתונים נשמרו בהצלחה!');

                if (type == 2)
                    alertMessage('הנתונים נשמרו בהצלחה ונשלחו למשרד להמשך טיפול!');

                if (type == 3) {

                    setTimeout(_openLink, 400);

                }

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





                var obj = this.worker;

                setRightDate(obj);



                if (IsInvalid) {

                    alertMessage("נא להכניס תאריך נכון בשדות תאריך!", 3);
                    return;
                }

                var Signature = $scope.accept();
                if (!Signature.isEmpty) {
                    obj["ImgData"] = Signature.dataUrl;
                } else {
                    obj["ImgData"] = "";

                }




                if (type == 1) {






                    // usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {
                    //  this.worker = worker;
                    SaveDynamicData(1, obj);


                    //}.bind(this));

                }

                if (type == 2) {
                    if (this.scope.workercampainsForm.$valid) {



                        //alert(this.worker.IsHaveSignature);
                        if ((this.campain.MustSign || !this.worker.IsHaveSignature) && Signature.isEmpty) {


                            alertMessage("חובה לחתום על מסמך זה!", 3);
                            return;

                        }
                      




                        //var Signature = $scope.accept();
                        //if (!Signature.isEmpty) {
                        //    obj["ImgData"] = Signature.dataUrl;
                        //} else {
                        //    obj["ImgData"] = "";

                        //}


                        SaveDynamicData(2, obj);



                        //usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {




                        //    if (worker.Status == "נשלח למשרד") {
                        //        SaveDynamicData(worker);
                        //        alertMessage('הנתונים נשלחו למשרד בהצלחה!');
                        //    }

                        //    else
                        //        alertMessage(worker.Status);

                        //}.bind(this));
                    }

                    else {
                        alertMessage("יש למלא את כל השדות המסומנים באדום , אלו שדות חובה", 3);

                    }


                }

                if (type == 3) {


                    //var Signature = $scope.accept();
                    //if (!Signature.isEmpty) {
                    //    obj["ImgData"] = Signature.dataUrl;
                    //} else {
                    //    obj["ImgData"] = "";

                    //}



                    //usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {

                    SaveDynamicData(3, obj);



                   
                    //$.unblockUI();

                    //$window.open(this.uploadsUri + "Workers/2/AllPdfTemp.pdf", '_blank');
                    //}.bind(this));

                }

            } catch (err) {
                alertMessage(err.message, 3);


            }

        }


        function _openLink() {

            //const today = new Date();
            //const timestamp = today.getTime(); //

            $window.open(self.uploadsUri + "Workers/" + self.worker.Id + "/" + self.campain.Id + "/AllPdfTemp.pdf", '_blank');

        }
    }



})();
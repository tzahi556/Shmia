(function () {

    var app = angular.module('app');

   // https://test.dgtracking.co.il/#/worker/1880/

    app.component('worker', {
        templateUrl: 'app/workers/worker.template.html?v=3',
        controller: WorkerController,
        bindings: {
            worker: '<',
            files: '<',
            childs: '<',
            cities: '<',
            banks: '<',
            banksbrunchs: '<',
            users: '<',
            screendata: '<',
            farm: '<'

        }
    });

    function WorkerController(usersService, farmsService, $scope, $state, sharedValues, filesService, $window, $timeout) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        this.submit = _submit.bind(this);
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        // this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRole');
        this.farmStyle = localStorage.getItem('FarmStyle');
        this.farmid = localStorage.getItem('FarmId');
        
      
        this.uploadFile = _uploadFile.bind(this);
        this.uploadFileParud = _uploadFileParud.bind(this);
        this.uploadFileZikuyNeke = _uploadFileZikuyNeke.bind(this);
        this.uploadFileZikuyToshav = _uploadFileZikuyToshav.bind(this);
        this.uploadFileZikuyOleHadash = _uploadFileZikuyOleHadash.bind(this);
        this.uploadFileZikuyPsakDinMezonot = _uploadFileZikuyPsakDinMezonot.bind(this);
        this.uploadFileZikuyLuladMugbalut = _uploadFileZikuyLuladMugbalut.bind(this);
        this.uploadFileZikuyHayalEnd = _uploadFileZikuyHayalEnd.bind(this);
        this.uploadFileZikuyToarAkdemi = _uploadFileZikuyToarAkdemi.bind(this);

        this.uploadFileTiumMasAproove = _uploadFileTiumMasAproove.bind(this);
        this.uploadFileTiummas = _uploadFileTiummas.bind(this);
        this.uploadFileTiumMasTlush = _uploadFileTiumMasTlush.bind(this);
        this.clearSignuture = _clearSignuture.bind(this);


        this.removeFile = _removeFile.bind(this);
        this.init = _init.bind(this);
        this.changeBank = _changeBank.bind(this);

        this.removeChild = _removeChild.bind(this);
        this.addNewChild = _addNewChild.bind(this);
        this.saveWorker = _saveWorker.bind(this);
        this.getFileName = _getFileName.bind(this);
        this.changeDateFormat = _changeDateFormat.bind(this);

        

        this.uploadsUri = sharedValues.apiUrl + '/uploads/'
      
        this.foldertaz = "taz";

        self = this;
        this.fileparud = "";

        // this.childs = [];

        this.ImageSignuture;
       // this.image;
        this.init();



        $scope.getGroupsDetails = function (groupId) {

          
            return self.screendata.filter(x => x.f2g.FieldsGroupsId == groupId);


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

                if (this.screendata[i].f2g.FieldsDataTypesId == 4 && this.screendata[i].f2gwd.Value)
                {
                    this.screendata[i].f2gwd.Value = eval(this.screendata[i].f2gwd.Value);
                }


                if (this.screendata[i].f2g.FieldsDataTypesId == 3 && this.screendata[i].f2gwd.Value) {
                    
                    this.screendata[i].f2gwd.Value = new Date(moment(this.screendata[i].f2gwd.Value).format("YYYY-MM-DD"));

                    
                }

            }


            //debugger
           // var obj = this.worker;


            if (!this.worker.w.FarmId)
                 this.worker.farmid = this.farmid;

         //   this.image = this.uploadsUri + "/" + this.worker.Id + "/Signature.png";
            
            if ((this.worker.w101 && !this.worker.w101.ShnatMas))
                this.worker.w101.ShnatMas = moment().format('YYYY');

               
            if (this.worker.w) setDateForArray(this.worker.w);
            if (this.worker.w101) setDateForArray(this.worker.w101);
            //$('.date').inputmask("datetime", {
            //    mask: "1/2/y",
            //    placeholder: "dd/mm/yyyy",
            //    leapday: "-02-29",
            //    separator: "/",
            //    alias: "dd/mm/yyyy"
            //});


        
            setTimeout(function () {
                

                autocomplete(document.getElementById("txtCity"), $scope.$ctrl.cities);

                var allbanks = $scope.$ctrl.banks;

                for (var i in allbanks) {
                    allbanks[i].Name = allbanks[i].Name + " - " + allbanks[i].Id ;

                }

                autocomplete(document.getElementById("txtBanks"), allbanks);

                
               // if ($scope.$ctrl.worker.BankNumName) { $scope.$ctrl.changeBank();}

                
            }, 1000);


           
              

            //}.bind(this));



          


            this.tazfiles = this.getFileName(1);// this.files.filter(x => x.Type == 1)[0].FileName;
            this.fileparud = this.getFileName(2);
            this.filezikuyneke = this.getFileName(3);
            this.filezikuytoshav = this.getFileName(4);
            this.filezikuyolehadash = this.getFileName(5);
            this.filezikuypsakdinmezonot = this.getFileName(6);
            this.filezikuyluladmugbalut = this.getFileName(7);
            this.filezikuyhayalend = this.getFileName(8);

            this.filezikuytoarakdemi = this.getFileName(9);
            this.filetiummasaproove = this.getFileName(10);
            this.filetiummas = this.getFileName(11);

            this.filetiummastlush = this.getFileName(12);

           

            // הכנסה של החתימה שתהיה בדף תמיד
            //var myCanvas = document.getElementById('maincanvas');
            //var ctx = myCanvas.getContext('2d');
            //var img = new Image;
          
            //img.onload = function () {
            //    ctx.drawImage(img, 0, 0); // Or at whatever offset you like
            //};

            //img.src = this.uploadsUri + "/" + this.worker.Id + "/Signature.png";
          

           
        }

        

        function _changeBank() {


           

            this.worker.BrunchNumName = "";
            var allbanksbrunchs = [];

            var selectedBank = this.banks.filter(x => x.Name == this.worker.BankNumName);

            if (selectedBank.length > 0) {

                allbanksbrunchs = angular.copy(this.banksbrunchs.filter(x => x.BankId == selectedBank[0].Id));

                for (var i in allbanksbrunchs) {
                    allbanksbrunchs[i].Name = allbanksbrunchs[i].Id + " - " + allbanksbrunchs[i].Name;
                }

                autocomplete(document.getElementById("txtBanksBrunchs"), allbanksbrunchs);

            }


            //for (var i in this.banks) {
                
            //    var BankNumName = this.banks[i].Name;

            //    if (BankNumName == this.worker.BankNumName) {

            //        debugger
            //        allbanksbrunchs = this.banksbrunchs.filter(x => x.BankId == this.banks[i].Id);

            //        for (var i in allbanksbrunchs) {
            //            allbanksbrunchs[i].Name = allbanksbrunchs[i].Id + " - " + allbanksbrunchs[i].Name;
            //        }

            //        autocomplete(document.getElementById("txtBanksBrunchs"),allbanksbrunchs);
                 

            //        break;
            //    }

            //}
      //  alert(this.worker.BankNumName);

        }

        function _getFileName(type) {


            if (type == 1) {

                var rows = this.files.filter(x => x.Type == type);
                if (rows.length == 0) return [];
                else return rows;

            }


            var rows = this.files.filter(x => x.Type == type);
            if (rows.length == 0) return "";
            else
                return rows[0].FileName;
        }

        function _uploadFile(file) {


           
            if (file == "NoTaz") {

                alertMessage("יש למלא תעודה זהות בשדה המיועד לכך!");
                return;
            }


            if (!file) {

                alertMessage("יש להעלות תעודה זהות בהירה ותקינה!");

                return;
            }

            this.tazfiles = this.tazfiles || [];
            var allfiles = file.split(",") || [];

          
            for (var i in allfiles) {
                var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
                this.files.push(Obj);
                this.tazfiles.push(Obj);
            }
            
        }

        function _uploadFileParud(file) {

            if (file) {

                var allCurrentFiles = this.files.filter(x => x.Type == 2);
                for (var i in allCurrentFiles) {
                    // if (this.files[i].FileName == file) {

                    allCurrentFiles.splice(i, 1);

                    // }
                }


                this.fileparud = file;
                var Obj = { "Id": 0, "Type": 2, "FileName": file };
                this.files.push(Obj);

            }

        }

        function _uploadFileZikuyNeke(file) {

            if (file) {

                this.filezikuyneke = file;
                this.files = this.files.filter(x => x.Type != 3);
                var Obj = { "Id": 0, "Type": 3, "FileName": file };
                this.files.push(Obj);

            }

        }

        function _uploadFileZikuyToshav(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 4);
                this.filezikuytoshav = file;
                var Obj = { "Id": 0, "Type": 4, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyOleHadash(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 5);
                this.filezikuyolehadash = file;
                var Obj = { "Id": 0, "Type": 5, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyPsakDinMezonot(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 6);
                this.filezikuypsakdinmezonot = file;
                var Obj = { "Id": 0, "Type": 6, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyLuladMugbalut(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 7);
                this.filezikuyluladmugbalut = file;
                var Obj = { "Id": 0, "Type": 7, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyHayalEnd(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 8);
                this.filezikuyhayalend = file;
                var Obj = { "Id": 0, "Type": 8, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyToarAkdemi(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 9);
                this.filezikuytoarakdemi = file;
                var Obj = { "Id": 0, "Type": 9, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiumMasAproove(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 10);
                this.filetiummasaproove = file;
                var Obj = { "Id": 0, "Type": 10, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiummas(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 11);
                this.filetiummas = file;
                var Obj = { "Id": 0, "Type": 11, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiumMasTlush(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 12);
                this.filetiummastlush = file;
                var Obj = { "Id": 0, "Type": 12, "FileName": file };
                this.files.push(Obj);


            }

        }



        function _clearSignuture() {

            //var canvas = document.getElementById("bcPaintCanvas");
            //var context = canvas.getContext('2d');
            //context.clearRect(0, 0, canvas.width, canvas.height); //clear html5 canvas

            // $('#bcPaintCanvas').html('');
            //  $.fn.bcPaint.clearCanvas();


            $scope.clear();

            //  $.fn.bcPaint.export();
        }



        function _addNewChild() {

            if (!this.newChild || !this.newChild.Name || !this.newChild.BirthDate) {

                alertMessage("יש למלא שדות תיקניים!");
                return;
            }

            if (!this.newChild.Taz || ValidateID(this.newChild.Taz) != 1) {

                alertMessage("שדה תעודת זהות אינו תקין!");
                return;
            }




           
            
            var childBirthDate = this.changeDateFormat(this.newChild.BirthDate); 

          

            this.childs = this.childs || [];
            this.childs.push({ Id: this.newChild.Id, Name: this.newChild.Name, Taz: this.newChild.Taz, BirthDate: childBirthDate, IsInHouse: this.newChild.IsInHouse, IsBituaLeumi: this.newChild.IsBituaLeumi });


            this.newChild.Name = "";
            this.newChild.Taz = "";
            this.newChild.BirthDate = "";
            this.newChild.IsInHouse = false;
            this.newChild.IsBituaLeumi = false;


            // this.initNewHorse();
        }

        function _removeChild(child) {

            for (var i in this.childs) {
                if (this.childs[i].Id == child.Id) {
                    this.childs.splice(i, 1);
                }
            }
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


        function _removeFile(file, type) {
            filesService.delete(file).then(function () {
                if (type == 1)
                    for (var i in this.files) {
                        if (this.files[i].FileName == file) {

                            this.files.splice(i, 1);
                            this.tazfiles.splice(i, 1);
                            break;
                        }
                    }




            }.bind(this));
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

                var obj101 = this.worker.w101;//angular.copy(this.worker.w101);

                setRightDate(obj101);

                if (IsInvalid) {


                    alertMessage("נא להכניס תאריך נכון בשדות תאריך!", 3);
                    return;
                }

                if (type == 1) {

                    if (this.scope.workerForm.$valid && this.tazfiles.length > 0) {

                        obj101["IsValid"] = true;


                    } else {
                        obj101["IsValid"] = false;
                         
                    }

                   

                    var Signature = $scope.accept();
                    if (!Signature.isEmpty) {
                        obj101["ImgData"] = Signature.dataUrl;
                    } else {
                        obj101["ImgData"] = "";

                    }

                

                    usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {
                        //  this.worker = worker;
                        SaveDynamicData(worker);
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


                       // $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ  PDF ושולחים אותו למשרד <br/>אנא המתנ/י...</div></h5>' });



                       

                        usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {



                            $.unblockUI();
                            if (worker.Status == "נשלח למשרד") {
                                SaveDynamicData(worker);
                                alertMessage('הנתונים נשלחו למשרד בהצלחה!');
                            }
                                
                            else
                                alertMessage(worker.Status);

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



                    usersService.updateWorker(this.worker, this.files, this.childs, type).then(function (worker) {
                        
                        SaveDynamicData(worker);
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
(function () {

    var app = angular.module('app');



    app.component('workernew', {
        templateUrl: 'app/workersnew/workernew.template.html?v=3',
        controller: WorkernewController,
        bindings: {
            workernew: '<',
            files: '<',
            childs: '<',
            cities: '<',
            banks: '<',
            banksbrunchs: '<'

        }
    });

    function WorkernewController(usersService, $scope, $state, sharedValues, filesService, $window, $timeout) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        this.submit = _submit.bind(this);
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        // this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRole');

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

     
        this.fileparud = "";

        // this.childs = [];

        this.ImageSignuture;
       // this.image;
        this.init();
        function _init() {
          
            var obj = this.workernew;

            if (!this.workernew.ShnatMas)
               this.workernew.ShnatMas = moment().format('YYYY');
           
         //   this.image = this.uploadsUri + "/" + this.workernew.Id + "/Signature.png";

            Object.keys(obj).forEach(function (key, index) {

              

                if (key.indexOf("Date") != -1 && obj[key] && key != "DateRigster" ) {

                   

                    obj[key] = moment(obj[key]).format("DD/MM/YYYY");// .startOf('day').toDate();

                }

               

            });


            $('.date').inputmask("datetime", {
                mask: "1/2/y",
                placeholder: "dd/mm/yyyy",
                leapday: "-02-29",
                separator: "/",
                alias: "dd/mm/yyyy"
            });


        
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

            //img.src = this.uploadsUri + "/" + this.workernew.Id + "/Signature.png";
          
        }

        

        function _changeBank() {


           

            this.workernew.BrunchNumName = "";
            var allbanksbrunchs = [];

            var selectedBank = this.banks.filter(x => x.Name == this.workernew.BankNumName);

            if (selectedBank.length > 0) {

                allbanksbrunchs = angular.copy(this.banksbrunchs.filter(x => x.BankId == selectedBank[0].Id));

                for (var i in allbanksbrunchs) {
                    allbanksbrunchs[i].Name = allbanksbrunchs[i].Id + " - " + allbanksbrunchs[i].Name;
                }

                autocomplete(document.getElementById("txtBanksBrunchs"), allbanksbrunchs);

            }


            //for (var i in this.banks) {
                
            //    var BankNumName = this.banks[i].Name;

            //    if (BankNumName == this.workernew.BankNumName) {

            //        debugger
            //        allbanksbrunchs = this.banksbrunchs.filter(x => x.BankId == this.banks[i].Id);

            //        for (var i in allbanksbrunchs) {
            //            allbanksbrunchs[i].Name = allbanksbrunchs[i].Id + " - " + allbanksbrunchs[i].Name;
            //        }

            //        autocomplete(document.getElementById("txtBanksBrunchs"),allbanksbrunchs);
                 

            //        break;
            //    }

            //}
      //  alert(this.workernew.BankNumName);

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
            this.tazfiles = this.tazfiles || [];
            var allfiles = file.split(",") || [];


            for (var i in allfiles) {
                var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
                this.files.push(Obj);
                this.tazfiles.push(Obj);
            }
            // 
            //if (file) {
            //    var Obj = { "Id": 0, "HorseId": 22, "FileName": file };
            //    this.files.push(Obj);

            //}
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



            var dd = dateVal.substring(0, 2);
            var mm = dateVal.substring(3, 5);
            var yy = dateVal.substring(6, 11);



            return yy + "-" + mm + "-" + dd;

        }

        function _saveWorker(type) {

           
            try {

                var thisCtrl = this;

                var obj = angular.copy(this.workernew);

                Object.keys(obj).forEach(function (key, index) {

                    if (key.indexOf("Date") != -1 && obj[key] && key != "DateRigster") {

                        obj[key] = thisCtrl.changeDateFormat(obj[key]);


                        //  obj[key].setHours((obj[key]).getHours() + 3);


                    }

                });

              

                if (type == 1) {



                    //$.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע שומרים את הנתונים  <br/>אנא המתנ/י...</div></h5>' });

                    var Signature = $scope.accept();
                    if (!Signature.isEmpty) {
                        obj["ImgData"] = Signature.dataUrl;
                    } else {
                        obj["ImgData"] = "";

                    }

                    //var paintCanvas = document.getElementById('bcPaintCanvas');
                    //var imgData = paintCanvas.toDataURL('image/png');
                    //obj["ImgData"] = imgData;

                    //// אם זה ריק אל תשלח כלום
                    //if (imgData.indexOf("ECBAgQIEDgAQNaAJ+CzbUNAAAAAElFTkSuQmCC") != -1)
                    //    obj["ImgData"] = "";

                    usersService.updateWorker(obj, this.files, this.childs, type).then(function (worker) {
                        //  this.workernew = worker;
                        alertMessage('הנתונים נשמרו בהצלחה!');
                        // $.unblockUI();
                        //
                    }.bind(this));

                }

                if (type == 2) {


                    if (this.scope.workerForm.$valid) {
                      
                        var Signature = $scope.accept();
                        if (!Signature.isEmpty) {
                            obj["ImgData"] = Signature.dataUrl;
                        } else {
                            obj["ImgData"] = "";

                        }


                        $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ  PDF ושולחים אותו למשרד <br/>אנא המתנ/י...</div></h5>' });

                        usersService.updateWorker(obj, this.files, this.childs, type).then(function (worker) {

                            $.unblockUI();
                            if (worker.Status == "נשלח למשרד")
                                alertMessage('הנתונים נשלחו למשרד בהצלחה!');
                            else
                                alertMessage(worker.Status);

                        }.bind(this));
                    }

                    else {
                        alertMessage("יש למלא את כל השדות המסומנים באדום , אלו שדות חובה");

                    }


                }

                if (type == 3) {
                    $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ PDF  <br/>אנא המתנ/י...</div></h5>' });

                    var Signature = $scope.accept();
                    if (!Signature.isEmpty) {
                        obj["ImgData"] = Signature.dataUrl;
                    } else {
                        obj["ImgData"] = "";

                    }



                    usersService.updateWorker(obj, this.files, this.childs, type).then(function (worker) {

                        $.unblockUI();
                        $window.open(this.uploadsUri + "/" + this.workernew.Id + "/"+ obj.UniqNumber + ".pdf", '_blank');

                    }.bind(this));

                }
            } catch (err) {

                alert(err.message);

            }

        }

        function _submit() {

        }

        function _delete() {

            var dd = this.workernew;
            if (confirm('האם למחוק את המשתמש?')) {
                usersService.deleteUser(this.user.Id).then(function (res) {
                    $state.go('workers');
                });
            }
        }


    }



})();
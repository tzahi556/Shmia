(function () {
    var app = angular.module('app');

    app.component('farmmanager', {
        templateUrl: 'app/farms/farmmanager.template.html',
        controller: FarmmanagerController,
        bindings: {
            farmmanager: '<',
            farm: '<',
            farmspdffiles: '<',
            btns: '<',
            grps: '<',
            btns2grps: '<'
        }
    });
    app.filter('dateRangeClalit', function () {

        return dateRangeFilter;

        function dateRangeFilter(items, from, to) {
            var results = [];
            var fromDate = moment(from).format('YYYYMMDD');
            var toDate = moment(to).format('YYYYMMDD');
            for (var i in items) {
                var startDate = moment(items[i].HalivaDate).format('YYYYMMDD');

                if (startDate >= fromDate && startDate <= toDate) {
                    results.push(items[i]);
                }
            }
            return results;
        }
    });
    function FarmmanagerController($scope, farmsService, filesService, $state, sharedValues, $http) {


        var self = this;

        $scope.farm = this.farm;
        this.scope = $scope;

        //this.horsesService = horsesService;
        this.farmsService = farmsService;
        this.SaveData = _SaveData.bind(this);
        this.uploadFile = _uploadFile.bind(this);
        this.uploadFilePdf = _uploadFilePdf.bind(this);
        this.deleteLogo = _deleteLogo.bind(this);
        this.actionFieldGroup = _actionFieldGroup.bind(this);

        this.init = _init.bind(this);
        this.role = localStorage.getItem('currentRole');
        //this.IsPopUp = false;
        //this.IsStop = false;

        function _deleteLogo(type) {


            if (type == 1)

                filesService.delete(self.LogoTemp).then(function () {

                    self.farm.Logo = "../../images/default-avatar.png";


                }.bind(this));

            if (type == 2)

                filesService.delete(self.SignTemp).then(function () {

                    self.farm.Sign = "../../images/empty.png";


                }.bind(this));

        }

        var getUrlParameter = function getUrlParameter(sParam) {


            var sPageURL = window.location.href;

            if (sPageURL.indexOf("fromSend") != -1) return true;

            else return false;

        };

        function _init() {

           
            self.farm.Style = (self.farm.Style) ? self.farm.Style.toString() : "0";




            if (self.farm.Logo && self.farm.Logo.indexOf("http") == -1) {
                self.LogoTemp = "/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo;
                self.farm.Logo = sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo;
            } else if (!self.farm.Logo) {

                self.LogoTemp = "";
                self.farm.Logo = "../../images/default-avatar.png";
            }



            if (self.farm.Sign && self.farm.Sign.indexOf("http") == -1) {
                self.SignTemp = "/Companies/" + self.farm.Id + "/PDFS/" + self.farm.Sign ;
                self.farm.Sign =  sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/PDFS/" + self.farm.Sign + "?" + (new Date()).getTime();
            } else if (!self.farm.Sign) {

                self.SignTemp = "";
                self.farm.Sign = " ../../images/empty.png";
            }

           



            for (var i in this.farmspdffiles) {

                var f = this.farmspdffiles[i];


                f.FullLink = sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/PDFS/" + f.FileName;

                if (eval(f.Is101)) f.FullLink = sharedValues.apiUrl + "/Uploads/Companies/101.pdf";


            }

            //debugger
            //var sddsds = this.farmspdffiles;

        }

        $scope.makeDropPdf = function (Id) {
            // debugger
            //var sddsds = self.farmspdffiles;


            var confirmBox = alertMessage("האם אתה בטוח שברצונך להסיר את הקובץ?", 4);
            confirmBox.click(function () {


                farmsService.updateFarmsPdfFiles(3, Id, self.farmspdffiles).then(function (farmspdffiles) {

                    self.farmspdffiles = farmspdffiles;
                    self.init();

                    const myTimeout = setTimeout(RefreshPage, 300);
                    /*   RefreshPage();*/


                    //self.init();

                    //$state.reload();

                });

            })
            //alert($scope.farm.Id);
        }

        this.init();

        function _uploadFilePdf(file) {

            farmsService.getFarmPDFFiles(this.farm.Id).then(function (farmspdffiles) {



                self.farmspdffiles = farmspdffiles;
                self.init();
                const myTimeout = setTimeout(RefreshPage, 300);



            });


        }

        function _uploadFile(file) {


            farmsService.getFarm(this.farm.Id).then(function (farm) {

                self.farm = farm;

                self.init();

            });


            //var allfiles = file.split(",") || [];


            //for (var i in allfiles) {
            //    var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
            //    this.files.push(Obj);
            //    this.tazfiles.push(Obj);
            //}

        }

        $scope.sendNewOrderPdfs = function (IdsOrders) {
            // debugger


            // שליחה של הסדר החדש

            var farmspdffilestemp = [];

            for (var i = 0; i < IdsOrders.length; i++) {


                var CurrentPdf = self.farmspdffiles.filter(x => x.Id == IdsOrders[i]);

                if (CurrentPdf.length > 0) {

                    CurrentPdf[0].Seq = i + 1;

                    farmspdffilestemp.push(CurrentPdf[0]);

                }


            }

            farmsService.updateFarmsPdfFiles(4, this.farm.Id, farmspdffilestemp).then(function (farmspdffiles) {

                //this.farmspdffiles = farmspdffiles;
                const myTimeout = setTimeout(RefreshComboFiles, 300);
               
            });

        }

        $scope.sendNewOrderforGroupsFields = function (type, IdsOrders) {
            // debugger
            var grpstemp = [];

            for (var i = 0; i < IdsOrders.length; i++) {


                var grp = self.grps.filter(x => x.Id == IdsOrders[i]);

                if (grp.length > 0) {

                    grp[0].Seq = i + 1;

                    grpstemp.push(grp[0]);

                }


            }

            farmsService.actionFieldGroup(8, this.farm.Id, grpstemp).then(function (grps) {

                self.grps = grps;
                const myTimeout = setTimeout(BuildEditPDF, 300);
            });

        }

        $scope.sendFields2Groups = function (type, grpId, btnid) {


            var existfield2group = self.btns2grps.filter(x => x.f2g.FieldsGroupsId == grpId && x.f2g.FieldsId == btnid);

            if (existfield2group.length > 0) {

                alertMessage("שדה קיים בקבוצה!", 3);
                return;
            }


            var Objects = { FieldsId: btnid, FieldsGroupsId: grpId };

            farmsService.actionFieldGroup(9, self.farm.Id, Objects).then(function (btns2grps) {


                self.btns2grps = btns2grps;
                const myTimeout = setTimeout(BuildEditPDF, 300);




            });

        }

        $scope.getSetFields2PDF = function (type, Objects) {

            // שליפה של כל הנקודות
            if (type == 1) {
                farmsService.actionFieldGroup(13, self.farm.Id, Objects).then(function (field2pdf) {

                    (async () => {

                        while (!CurrentScale && self.farmspdffiles.length>0) // define the condition as you like
                            await new Promise(resolve => setTimeout(resolve, 500));
                       
                        AddParamsToPDF(field2pdf);
                       
                      
                    })();
                   

                   

                });

            }

            // עדכון או הוספה או מחיקה
            if (type == 2) {
                farmsService.actionFieldGroup(14, self.farm.Id, Objects).then(function (field2pdf) {

                    (async () => {

                        while (!CurrentScale && self.farmspdffiles.length > 0) // define the condition as you like
                            await new Promise(resolve => setTimeout(resolve, 500));
                       
                        AddParamsToPDF(field2pdf);


                    })();


                    //setTimeout(function () {

                    //    AddParamsToPDF(field2pdf);
                    //}, 400);

                });







            }



        }

        $scope.FilterOnlySign = function (item) {
           
            return item.WorkerTableField === '1' || item.WorkerTableField === '2';
        };

        $scope.FilterWithoutSign = function (item) {

            return item.WorkerTableField != '1' && item.WorkerTableField != '2';
        };

        $scope.setNewPdfGenerator = function () {

            farmsService.actionFieldGroup(15, self.farm.Id, null).then(function (field2pdf) {

                //(async () => {

                //    while (!CurrentScale && self.farmspdffiles.length > 0) // define the condition as you like
                //        await new Promise(resolve => setTimeout(resolve, 500));
                //    AddParamsToPDF(field2pdf);


                //})();






            });

        }


        


        function _SaveData(type) {



            //שמירת פרטי חברה
            if (type == 1) {

                this.farmsService.updateFarm(this.farm).then(function (farm) {
                    alertMessage('הנתונים נשמרו בהצלחה!', 1);

                }.bind(this));


            }


            // הוספת קובץ 101 בלבד
            if (type == 2) {



                farmsService.updateFarmsPdfFiles(2, self.farm.Id, self.farmspdffiles).then(function (farmspdffiles) {




                    self.farmspdffiles = farmspdffiles;
                    self.init();
                    const myTimeout = setTimeout(RefreshPage, 300);

                    // 

                    // $state.reload();

                });


            }

            //עריכת קבוצה
            if (type == 3) {


                var Objects = this.grp;
                this.farmsService.actionFieldGroup(11, this.farm.Id, Objects).then(function (grps) {

                    self.grps = grps;
                    const myTimeout = setTimeout(BuildEditPDF, 300);

                }.bind(this));


            }

            //עריכת שדה
            if (type == 4) {


                var Objects = this.btns2grp;
                this.farmsService.actionFieldGroup(12, this.farm.Id, Objects).then(function (btns2grps) {

                    self.btns2grps = btns2grps;
                    const myTimeout = setTimeout(BuildEditPDF, 300);

                }.bind(this));


            }




        }

        function _actionFieldGroup(type, objid) {

            var thisCtrl = this;

            //הוספה שדה
            if (type == 4) {

                if (!this.FieldName) {

                    alertMessage("שם שדה הינו שדה חובה!", 3);
                    return;
                }

                var Objects = { Obj: this.FieldName };

                this.farmsService.actionFieldGroup(4, this.farm.Id, Objects).then(function (btns) {



                    if (btns == "exist obj") {

                        alertMessage("שם שדה קיים כבר במערכת!", 3);
                        return;

                    }


                    self.btns = btns;
                    const myTimeout = setTimeout(BuildEditPDF, 300);


                    // self.init();
                    // const myTimeout = setTimeout(RefreshPage, 300);
                }.bind(this));

            }

            //מחיקת שדה
            if (type == 5) {

                var Objects = { Obj: objid };
                var confirmBox = alertMessage("האם אתה בטוח שברצונך להסיר את השדה?", 4);
                confirmBox.click(function () {

                    thisCtrl.farmsService.actionFieldGroup(5, thisCtrl.farm.Id, Objects).then(function (btns) {

                        self.btns = btns;

                        const myTimeout = setTimeout(BuildEditPDF, 300);


                    }.bind(this));


                })

            }

            //הוספה קבוצה
            if (type == 6) {


                if (!this.GroupName) {

                    alertMessage("שם קבוצה הינו שדה חובה!", 3);
                    return;
                }
                var Objects = { Obj: this.GroupName };

                this.farmsService.actionFieldGroup(6, this.farm.Id, Objects).then(function (grps) {

                    self.grps = grps;
                    const myTimeout = setTimeout(BuildEditPDF, 300);

                }.bind(this));


            }

            //מחיקת קבוצה
            if (type == 7) {

                var Objects = { Obj: objid };
                var confirmBox = alertMessage("האם אתה בטוח שברצונך להסיר את הקבוצה?", 4);
                confirmBox.click(function () {

                    thisCtrl.farmsService.actionFieldGroup(7, thisCtrl.farm.Id, Objects).then(function (grps) {

                        self.grps = grps;

                        const myTimeout = setTimeout(BuildEditPDF, 300);


                    }.bind(this));


                })

            }

            //מחיקת שדה מתוך קבוצה
            if (type == 10) {

                var Objects = { Obj: objid };
                var confirmBox = alertMessage("האם אתה בטוח שברצונך להסיר את השדה מהקבוצה?", 4);
                confirmBox.click(function () {

                    thisCtrl.farmsService.actionFieldGroup(10, thisCtrl.farm.Id, Objects).then(function (btns2grps) {



                        thisCtrl.btns2grps = btns2grps;

                        const myTimeout = setTimeout(BuildEditPDF, 300);


                    }.bind(this));


                })

            }

            //עריכת קבוצה
            if (type == 11) {

                var grp = this.grps.filter(x => x.Id == objid);

                if (grp.length > 0) {
                    
                    thisCtrl.grp = grp[0];
                    thisCtrl.grp.CountFieldsInRow = grp[0].CountFieldsInRow.toString();
                    thisCtrl.grp.TitleTypeId = grp[0].TitleTypeId.toString();
                    OpenDialog(1);
                }



            }

            //עריכת שדה בקבוצה
            if (type == 12) {

                var btns2grp = this.btns2grps.filter(x => x.f2g.Id == objid);

                if (btns2grp.length > 0) {



                    thisCtrl.btns2grp = btns2grp[0].f2g;
                    thisCtrl.btns2grp.Name = btns2grp[0].f.Name;

                    thisCtrl.btns2grp.FieldsDataTypesId = btns2grp[0].f2g.FieldsDataTypesId.toString();
                    OpenDialog(2);
                }



            }

        }





    }

})();
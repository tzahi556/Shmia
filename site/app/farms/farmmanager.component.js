(function () {
    var app = angular.module('app');

    app.component('farmmanager', {
        templateUrl: 'app/farms/farmmanager.template.html',
        controller: FarmmanagerController,
        bindings: {
            farmmanager: '<',
            farm: '<',
            farmspdffiles: '<'
            //horses: '<',
            //horsegroups: '<',
            //horsegroupshorses: '<',
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
        this.delete = _delete.bind(this);

        //this.addNewTag = _addNewTag.bind(this);
        //   this.removeTags = _removeTags.bind(this);

        this.uploadFile = _uploadFile.bind(this);
        this.uploadFilePdf = _uploadFilePdf.bind(this);


        this.initNewTags = _initNewTags.bind(this);

        this.getKlalitHistoriPage = _getKlalitHistoriPage.bind(this);
        this.setKlalitHistoriPage = _setKlalitHistoriPage.bind(this);
        this.setPostToKlalit = _setPostToKlalit.bind(this);

        this.getHorsesGroup = _getHorsesGroup.bind(this);
        this.actionHorsesGroup = _actionHorsesGroup.bind(this);
        this.getFreeHorses = _getFreeHorses.bind(this);
        this.checkAll = _checkAll.bind(this);

        this.deleteLogo = _deleteLogo.bind(this);


        this.init = _init.bind(this);
        this.role = localStorage.getItem('currentRole');

        this.IsPopUp = false;
        this.IsStop = false;

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

        function _setKlalitHistoriPage(type, klalitId) {
            this.IsStop = false;

            if (this.klalitsBefore.length > 0) {

                // $('#modalKlalitSend').modal('show');
                var fromSend = getUrlParameter("fromSend");
                if (fromSend) {


                    $('#modalKlalitSend').modal('show');
                    var ctrlthis = this;
                    ctrlthis.endMessage = false;
                    ctrlthis.currentElement = 0;

                    for (var i in ctrlthis.klalitsBefore) {

                        ctrlthis.klalitsBefore[i].ResultXML = null;
                        ctrlthis.klalitsBefore[i].FirstName = ctrlthis.klalitsBefore[i].FirstName;
                        ctrlthis.klalitsBefore[i].LastName = ctrlthis.klalitsBefore[i].LastName;


                    }

                    ctrlthis.setPostToKlalit(0, ctrlthis);

                } else {
                    //https://www.giddyup.co.il/#/farmmanager/?fromSend=fromSend
                    //http://localhost:51517/#/farmmanager/?fromSend=fromSend
                    var payWind = window.open("https://www.giddyup.co.il/#/farmmanager/?fromSend=fromSend", "Upload Chapter content", "width=700,height=350,top=200,left=500");


                }


            }




        }

        function _setPostToKlalit(index, ctrlthis) {


            if (this.IsStop) {

                alert("השידור לכללית נעצר!");
                return;
            }

            $http.post(sharedValues.apiUrl + 'farms/setKlalitHistoris/', ctrlthis.klalitsBefore[index]).then(function (response) {

                ctrlthis.returnFirstName = response.data.FirstName;
                ctrlthis.returnLastName = response.data.LastName;

                ctrlthis.returnDate = response.data.DateLesson;
                ctrlthis.returnStatus = response.data.Result;
                ctrlthis.currentElement = index + 1;

                if ((index + 1) < ctrlthis.klalitsBefore.length)
                    ctrlthis.setPostToKlalit(index + 1, ctrlthis);
                else {
                    ctrlthis.endMessage = true;

                    window.opener.$("#btnSearch").click();
                    //window.opener.$scope.getKlalitHistoriPage();
                    //ctrlthis;
                }


            });

        }

        function _checkAll() {

            //  this.klalits.find(x => x.ResultNumber == -1).IsDo = this.checkAllc;



            this.klalits.filter(x => x.ResultNumber != 1)
                .forEach(x => x.IsDo = this.checkAllc);
        }

        function _getKlalitHistoriPage(type, klalitId) {


            var startDate = moment(this.dateFromClalit).format('YYYY-MM-DD');
            var endDate = moment(this.dateToClalit).format('YYYY-MM-DD');

            // בחירת מסומנים
            if (type == 5) {

                this.klalitsBefore = this.klalits.filter(x => x.IsDo);
                if (this.klalitsBefore.length == 0) {
                    alert("עלייך לבחור רשומות לשליחה חוזרת!");
                    return;
                }


                farmsService.getKlalitHistoris(this.farmmanager.FarmId, startDate, endDate, 5, null, null).then(function (res) {
                    if (res[0] && res[0].Id == -1) {

                        this.klalitsBefore = this.klalitsBefore.slice(0, res[0].CounterSend);// סתם משתמש בשדה הזה לבדוק עוד כמה נותר לו


                        window.klalitsBefore = this.klalitsBefore;

                        if (this.klalitsBefore.length == 0) {

                            alert("עברת את מכסת התביעות האוטמטיות היומיות,ניתן לנסות מחר...");

                        } else {
                            $('#modalKlalit').modal('show');
                        }

                    }



                }.bind(this));


                return;

            }




            // שליחה עצמה
            //if (type == 2) {
            //    this.SaveData(2, true);
            //}

            // פתיחת חלון מקדים
            // if (type == 4) 

            farmsService.getKlalitHistoris(this.farmmanager.FarmId, startDate, endDate, type, klalitId, null).then(function (res) {

                if (type == 2 && res[0].Id < 0) {

                    if (res[0].Id == -1) alert("אין הגדרות למדריכים");
                    if (res[0].Id == -2) alert("אין הגדרות לחווה");
                    return;

                }

                if (type == 4) {

                    if (res[0] && res[0].Id == -1) {

                        alert("עברת את מכסת התביעות האוטמטיות היומיות,ניתן לנסות מחר...");
                        return;

                    } else {
                        this.klalitsBefore = res;
                        window.klalitsBefore = res;
                        $('#modalKlalit').modal('show');
                    }


                }
                else if (type == 1) {


                    if (res[0] && res[0].Id == -1) {

                        alert("עברת את מכסת התביעות האוטמטיות היומיות,ניתן לנסות מחר...");
                        return;

                    }

                    this.getKlalitHistoriPage(null, null);



                }

                else {

                    this.klalits = res;
                    for (var i in this.klalits) {

                        this.klalits[i].KlalitHistorisId = this.klalits[i].Id;
                        //  ctrlthis.klalitsBefore[i].UserName = ctrlthis.klalitsBefore[i].UserName.replace(' ', ',');
                        //

                    }


                }

            }.bind(this));

        }

        var getUrlParameter = function getUrlParameter(sParam) {


            var sPageURL = window.location.href;

            if (sPageURL.indexOf("fromSend") != -1) return true;

            else return false;

        };

        function _init() {
            //this.farm;
            //debugger

            //if (getUrlParameter("fromSend")) {

            //    this.IsPopUp = true;

            //    this.klalitsBefore = window.opener.klalitsBefore;

            //    this.setKlalitHistoriPage(4, 0);

            //}

            //   alert(self.farminstructors.length);

            //self.farm.Meta = self.farm.Meta || {};
            //self.farm.Meta.StartDate = self.farm.Meta.StartDate ? new Date(self.farm.Meta.StartDate) : null;
            //self.farm.Meta.EndDate = self.farm.Meta.EndDate ? new Date(self.farm.Meta.EndDate) : null;
            //self.farm.IsHiyuvInHashlama = (self.farm.IsHiyuvInHashlama) ? self.farm.IsHiyuvInHashlama.toString() : "0";

            //self.initNewTags();



           

            self.farm.Style = (self.farm.Style) ? self.farm.Style.toString() : "0";


            self.LogoTemp = (self.farm.Logo) ? "/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo : "";
            self.farm.Logo = (self.farm.Logo) ? sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/Logo/" + self.farm.Logo : "../../images/default-avatar.png";


            self.SignTemp = (self.farm.Sign) ? "/Companies/" + self.farm.Id + "/PDFS/" + self.farm.Sign : "";

            self.farm.Sign = (self.farm.Sign) ? sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/PDFS/" + self.farm.Sign + "?" + (new Date()).getTime() : "../../images/empty.png";


         
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

                //self.init();
               // $state.reload();

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



        //this.dateFromClalit = moment().add(0, 'months').toDate();
        //this.dateToClalit = moment().add(1, 'days').toDate();


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





        }

        function _delete() {
            if (confirm('האם למחוק את החווה?')) {
                farmsService.deleteFarm(this.farm.Id).then(function () {
                    $state.go('farms');
                });
            }
        }

        function _initNewTags() {

            self.farm.Meta.farmTags = self.farm.Meta.farmTags || [];
            self.newfarmTag = null;

            //if ($scope.newHorseForm != null) {
            //    $scope.newHorseForm.$setPristine();
            //}
        }

        ///**************************************************
        function _getHorsesGroup(id) {

            var res = this.horsegroupshorses.filter(x => x.HorseGroupsId == id);

            for (var i in res) {

                var horse = this.horses.filter(y => y.Id == res[i].HorseId);
                res[i].Name = horse[0].Name;
                res[i].ShoeingTimeZone = horse[0].ShoeingTimeZone;

            }

            return res;

        }

        $scope.makeDropHorse = function (newEvent, currentHorseId, currentGroupeId) {

            if (currentGroupeId == 0 || currentHorseId == 0) return;


            var currentHorsesList = $scope.$ctrl.getHorsesGroup(currentGroupeId);

            if (currentHorsesList.length > 0) {

                var FirstShoeingTimeZone = currentHorsesList[0].ShoeingTimeZone;
                var horse = $scope.$ctrl.horses.filter(y => y.Id == currentHorseId);
                if (horse[0].ShoeingTimeZone != FirstShoeingTimeZone) {


                    alert("לא ניתן לשייך פרקי זמן שונים בין פרזולים לאותה קבוצה...");
                    return;
                }


            }


            $scope.$ctrl.horsegroupshorses.push({ HorseGroupsId: currentGroupeId, HorseId: currentHorseId, Id: 0, FarmId: localStorage.getItem('FarmId') });




            $scope.$ctrl.horsesService.getSetHorseGroupsHorses(2, $scope.$ctrl.horsegroupshorses).then(function (res) {
                $scope.$ctrl.horsegroupshorses = res;
            }.bind(this));
        }

        function _actionHorsesGroup(type, obj) {

            var thisCtrl = this;
            //הוספה קבוצה
            if (type == 1) {

                if (!this.newGroup) {

                    alert("שדה שם קבוצה הינו שדה חובה!");
                    return;
                }

                this.horsegroups.push({ Name: this.newGroup, Id: 0, FarmId: localStorage.getItem('FarmId') });

                this.horsesService.getSetHorseGroups(2, this.horsegroups).then(function (res) {
                    thisCtrl.horsegroups = res;
                }.bind(this));

            }

            //מחיקת קבוצה
            if (type == 2) {
                for (var i in this.horsegroups) {
                    if (this.horsegroups[i] == obj) {


                        this.horsegroups.splice(i, 1);



                        var m = this.horsegroupshorses.length;
                        while (m--) {

                            if (this.horsegroupshorses[m].HorseGroupsId == obj.Id) {
                                this.horsegroupshorses.splice(m, 1);
                            }
                        }
                        //מחיקת הסוסים ששייכים לקבוצה
                        //for (var m in this.horsegroupshorses) {
                        //    if (this.horsegroupshorses[m].HorseGroupsId == obj.Id) {
                        //        this.horsegroupshorses.splice(m, 1);
                        //    }
                        //}

                    }
                }

                this.horsesService.getSetHorseGroups(2, this.horsegroups).then(function (res) {
                    thisCtrl.horsegroups = res;

                }.bind(this));



            }

            //מחיקת סוס
            if (type == 3) {
                for (var i in this.horsegroupshorses) {
                    if (this.horsegroupshorses[i].HorseId == obj) {
                        this.horsegroupshorses.splice(i, 1);
                    }
                }

                this.horsesService.getSetHorseGroupsHorses(2, this.horsegroupshorses).then(function (res) {
                    this.horsegroupshorses = res;
                }.bind(this));



            }


        }

        function _getFreeHorses() {

            var res = [];

            for (var i in this.horses) {

                if (this.horsegroupshorses.filter(x => x.HorseId == this.horses[i].Id).length > 0) continue;


                res.push(this.horses[i]);


            }

            return res;


        }



        //function _addNewTag() {


        //    //for (var i in this.userhorses) {
        //    //    if (this.userhorses[i].HorseId == this.newHorse.Id) {
        //    //        return false;
        //    //    }
        //    //}

        //    self.farm.Meta.farmTags.push({ tag_name: self.newfarmTag.tag_name, tag_id: self.newfarmTag.tag_id });
        //    //self.newFarmTags.tag_name = "";
        //    //self.newFarmTags.tag_id = "";
        //    self.initNewTags();
        //}

        //function _removeTags(tag) {
        //    var farmTags = self.farm.Meta.farmTags;
        //    for (var i in farmTags) {
        //        if (farmTags[i].tag_id == tag.tag_id && farmTags[i].tag_name == tag.tag_name) {
        //            farmTags.splice(i, 1);
        //        }
        //    }
        //}





    }

})();
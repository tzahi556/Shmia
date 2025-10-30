(function () {

    var app = angular.module('app');


    app.component('workers', {
        templateUrl: 'app/workers/workers.template.html?v=2',
        controller: WorkersController,
        bindings: {
            campain: '<',
            farm: '<',
            farmspdffiles: '<',
            //btns: '<',
            //grps: '<',
            //btns2grps: '<',
            workers: '<',
            departments: '<'

        }
    });

    function WorkersController($scope, farmsService, filesService, $state, sharedValues, $http) {


        var self = this;

        $scope.farm = this.farm;
        this.scope = $scope;
        this.checkAll = _checkAll.bind(this);
        this.sendSMS = _sendSMS.bind(this);

        this.farmsService = farmsService;
        this.SaveData = _SaveData.bind(this);

        this.init = _init.bind(this);
        this.role = localStorage.getItem('currentRolesId');


        this.currentFarm = JSON.parse(localStorage.getItem('FarmObj'));


        this.ShnatMas = moment().format('YYYY');








        function _init(isfromRefresh) {


            if (!isfromRefresh) {
                if (this.role == 2) {
                    this.active = 0;   // טאב ראשון
                } else {
                    this.active = 4;   // טאב עובדים (האחרון)
                }
            }



            if (localStorage.getItem('selectedTab') == "3" && this.campain.Name) {

                this.active = 4;

            }



            //******************************************************** */


            var workersFilters = JSON.parse(localStorage.getItem('workersFilters'));
            if (workersFilters) {
                self.statusid = workersFilters.statusid;
                self.ShnatMas = workersFilters.ShnatMas;
                self.departmentId0 = workersFilters.departmentId0;
                self.departmentId = workersFilters.departmentId;
                self.departmentId2 = workersFilters.departmentId2;
                self.departmentId3 = workersFilters.departmentId3;
                self.departmentId4 = workersFilters.departmentId4;
                self.filterText = workersFilters.filterText;



            } else {
                //// ערכים דיפולטיביים
                //vm.selectedYear = new Date().getFullYear();
                //vm.selectedStatus = '';
            }



            var campainLast = JSON.parse(localStorage.getItem('campainLast'));
            if (campainLast) {

                //$scope.$applyAsync(() => { self.campain = campainLast; });
                this.campain.Id = campainLast.Id;
                //$scope.$apply(() => {
                //    self.campain = campainLast;
                //});

            }



            if (this.campain.Name) {


                IsSave = true;

            }

            for (var i in this.farmspdffiles) {



                var f = this.farmspdffiles[i];


                f.FullLink = sharedValues.apiUrl + "/Uploads/Companies/" + self.farm.Id + "/PDFS/" + f.CampainsId + "/" + f.FileName;

                if (eval(f.Is101)) f.FullLink = sharedValues.apiUrl + "/Uploads/Companies/101.pdf";


            }

            var obj = this.campain;
            Object.keys(obj).forEach(function (key, index) {

                if (key.indexOf("Date") != -1) {

                    obj[key] = new Date(moment(obj[key]).format("YYYY-MM-DD"));// .startOf('day').toDate();


                }


            });


            // alert(this.campain.Id);


        }



        this.init();

        function _SaveData(type) {



            //שמירת פרטי חברה
            if (type == 1) {


                if (!this.campain.Name) {

                    alertMessage("שדה שם קמפיין הינו שדה חובה", 3)

                    return;
                }


                this.farmsService.getSetCampainsData(3, 0, this.campain).then(function (farm) {
                    alertMessage('הנתונים נשמרו בהצלחה!', 1);

                    IsSave = true;

                }.bind(this));


            }


            // הוספת קובץ 101 בלבד
            if (type == 2) {



                farmsService.updateFarmsPdfFiles(2, self.farm.Id, this.campain.Id, self.farmspdffiles).then(function (farmspdffiles) {




                    self.farmspdffiles = farmspdffiles;
                    self.init(true);
                    const myTimeout = setTimeout(RefreshPage, 300);



                });


            }

            //עריכת קבוצה
            if (type == 3) {


                var Objects = this.grp;
                this.farmsService.actionFieldGroup(11, this.farm.Id, Objects, this.campain.Id).then(function (grps) {

                    self.grps = grps;
                    const myTimeout = setTimeout(BuildEditPDF, 300);

                }.bind(this));


            }

            //עריכת שדה
            if (type == 4) {


                var Objects = this.btns2grp;
                this.farmsService.actionFieldGroup(12, this.farm.Id, Objects, this.campain.Id).then(function (btns2grps) {

                    self.btns2grps = btns2grps;
                    const myTimeout = setTimeout(BuildEditPDF, 300);

                }.bind(this));


            }




        }
        function _checkAll() {


            this.workers.Items.forEach(x => x.IsSelected = this.checkAllc);
        }
        function _sendSMS(type) {


            var ctrl = this;


            //alert(ctrl.campain.Id);


            //return;

            //var selected = this.workers.filter(x => x.IsSelected && (x.IsValid || this.farmStyle != 1));
            var selected = this.workers.Items.filter(x => x.IsSelected);

            if (selected.length == 0) {

                alertMessage(`לא נבחר עובד/ת למשלוח`, 3);

                return;
            }


            ctrl.checkAllc = false;
            ctrl.checkAll();


            for (var i = 0; i < selected.length; i++) {
                selected[i].IsSelected = true;
            }


            const workersSelected = selected.map(selected => selected.w);

            let typename = "SMS";

            if (type == 2) {

                typename = "מייל";
            }

            if (type == 3) {

                typename = "ווטסאפ";

                if (workersSelected.length == 1) {

                    const phone = "972" + workersSelected[0].PhoneSelular;

                    farmsService.sendLinktoWorkers(workersSelected, 4, ctrl.campain.Id).then(function (res) {

                        const message = res;

                        const encodedMessage = encodeURIComponent(message);

                        const url = `https://wa.me/${phone}?text=${encodedMessage}`;

                        window.open(url, '_blank');


                        farmsService.sendLinktoWorkers(workersSelected, type, ctrl.campain.Id).then(function (res) {

                            ctrl.workers = res;
                            ctrl.checkAllc = false;
                            ctrl.checkAll();
                        });

                    });

                }

                return;

            }

            if (selected.length > 0) {
                var confirmBox = alertMessage(`האם לשלוח ${typename} לכל העובדים המסומנים?`, 4);
                confirmBox.click(function () {

                    farmsService.sendLinktoWorkers(workersSelected, type, ctrl.campain.Id, ctrl.currentPage, ctrl.pageSize, ctrl.filterText, ctrl.statusid, ctrl.factoryid, ctrl.divisionid, ctrl.subdivisionid, ctrl.departmentsid, ctrl.subdepartmentsid, ctrl.status101, ctrl.ShnatMas).then(function (res) {

                        ctrl.workers = res;
                        ctrl.checkAllc = false;
                        ctrl.checkAll();
                    });



                });
            }



        }




        //********************************************* */

        this.currentPage = 1;
        this.pageSize = 10;

        this.getPagedWorkers = function () {

            var start = (this.currentPage - 1) * this.pageSize;
            return this.workers.Items.slice(start, start + this.pageSize);
        };

        this.totalPages = function () {
            return Math.ceil(this.workers.TotalCount / this.pageSize);
        };

        this.getPages = function () {
            const total = this.totalPages();
            const current = this.currentPage;
            const delta = 3; // כמה עמודים לפני ואחרי להציג

            const range = [];
            const rangeWithDots = [];
            let l;

            for (let i = 1; i <= total; i++) {
                if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
                    range.push(i);
                }
            }

            for (let i of range) {
                if (l) {
                    if (i - l === 2) {
                        rangeWithDots.push(l + 1);
                    } else if (i - l > 1) {
                        rangeWithDots.push('...');
                    }
                }
                rangeWithDots.push(i);
                l = i;
            }

            return rangeWithDots;
        };

        this.onSearchChange = function () {
            if (this.filterText && this.filterText.length >= 2) {
                this.currentPage = 1;

                // alert(this.filterText);
                this.loadWorkers();
            }

            // אם המשתמש מחק את הכל – נטען הכול מחדש (אופציונלי)
            if (!this.filterText || this.filterText.length === 0) {
                this.currentPage = 1;
                this.loadWorkers();
            }



            var workersFilters = {
                statusid: self.statusid,
                ShnatMas: self.ShnatMas,
                departmentId0: self.departmentId0,
                departmentId: self.departmentId,
                departmentId2: self.departmentId2,
                departmentId3: self.departmentId3,
                departmentId4: self.departmentId4,
                filterText: self.filterText

            };


            localStorage.setItem('workersFilters', JSON.stringify(workersFilters));

        };

        this.onShnatmasChange = function () {

            var ctrl = this;

            var statusid = this.statusid;
            var factoryid = this.departmentId0;
            var divisionid = this.departmentId;
            var subdivisionid = this.departmentId2;
            var departmentsid = this.departmentId3;
            var subdepartmentsid = this.departmentId4;
            var statuscampain = "";
            var shnatmas = this.ShnatMas;


            farmsService.getSetCampainsData(444, "-1", null, ctrl.currentPage, ctrl.pageSize, ctrl.filterText, statusid, factoryid, divisionid, subdivisionid, departmentsid, subdepartmentsid, statuscampain, shnatmas).then(function (res) {

                if (res) {
                    ctrl.campain = res;
                    localStorage.setItem('campainLast', JSON.stringify(ctrl.campain));
                }
            });

        };



        this.loadWorkers = function () {

            //alert(this.currentPage);

            var ctrl = this;

            var statusid = this.statusid;
            var factoryid = this.departmentId0;
            var divisionid = this.departmentId;
            var subdivisionid = this.departmentId2;
            var departmentsid = this.departmentId3;
            var subdepartmentsid = this.departmentId4;
            var statuscampain = "";
            var shnatmas = this.ShnatMas;



            farmsService.getSetCampainsData(44, ctrl.campain.Id, null, ctrl.currentPage, ctrl.pageSize, ctrl.filterText, statusid, factoryid, divisionid, subdivisionid, departmentsid, subdepartmentsid, statuscampain, shnatmas).then(function (res) {


                ctrl.workers = res;

            });




        }

        this.goToFirstPage = function () {
            if (this.currentPage > 1) {
                this.currentPage = 1;
                this.loadWorkers();
            }
        };

        this.goToLastPage = function () {
            if (this.currentPage < this.totalPages()) {
                this.currentPage = this.totalPages();
                this.loadWorkers();
            }
        };

        this.goToPage = function (n) {
            if (n !== '...' && n !== this.currentPage) {
                this.currentPage = n;
                this.loadWorkers();
            }
        };

        this.goToNextPage = function () {
            if (this.currentPage < this.totalPages()) {
                this.currentPage++;
                this.loadWorkers();
            }
        };

        this.goToPreviousPage = function () {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.loadWorkers();
            }
        };

        //********************************************** */






    }

})();
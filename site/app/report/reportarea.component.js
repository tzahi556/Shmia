(function () {

    var app = angular.module('app');



    app.component('reportarea', {
        templateUrl: 'app/report/reportarea.template.html',
        controller:ReportareaController,
        bindings: {
            reportarea: '<'

        }
    });

    function ReportareaController(usersService, $scope, $state, sharedValues, filesService, $window) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
       
        this.roles = usersService.roles;

        this.role = localStorage.getItem('currentRole');

    
        this.downloadData = _downloadData.bind(this);

        this.getHebArea = _getHebArea.bind(this);

        this.shnatmas = moment().format('YYYY');

        this.uploadsUri = sharedValues.apiUrl + '/uploads/Years_';

        this.upload = _upload.bind(this);
        this.sendToServer = _sendToServer.bind(this);
        this.startIndex = 0;
        function _upload() {


            //if (this.portfolios.length > 0) {

            //    var currentportfolios = this.portfolios.filter(x => x.Status != 3 && x.RowCount != 0);
            //    if (currentportfolios.length > 0) {

            //        alert("You Must Finish Work all Current Portfolios Before Upload New Excel");
            //        return;
            //    }

            //}

            var ctrlThis = this;
            function handleFile(e) {

               
                var files = e.target.files;
                var i, f;
                for (i = 0; i != files.length; ++i) {
                    f = files[i];
                    var reader = new FileReader();
                    var name = f.name;
                    reader.onload = function (e) {
                        var data = e.target.result;
                        var workbook;
                        workbook = XLSX.read(data, { type: 'binary' });
                       debugger
                        var portfolioitems = XLSX.utils.sheet_to_json(workbook.Sheets[workbook.SheetNames[0]]);





                        portfolioitems.map(function (portfolioitem) {
                            
                            portfolioitem.Id = 0;
                            portfolioitem.UserId = portfolioitem["מנהל"];
                            portfolioitem.UniqNumber = portfolioitem["עובד"];
                            portfolioitem.BirthDate = portfolioitem["תאריך לידה"];
                            portfolioitem.StartWorkDate = portfolioitem["תאריך התחלת עבודה"];
                            portfolioitem.FirstName = portfolioitem["שם פרטי"];
                            portfolioitem.LastName = portfolioitem["שם משפחה"];
                            portfolioitem.Taz = portfolioitem["תעודת זהות"];
                            portfolioitem.PhoneSelular = portfolioitem["טלפון"];
                            portfolioitem.City = portfolioitem["עיר"];
                            portfolioitem.Street = portfolioitem["כתובת"];

                            portfolioitem.Gender = portfolioitem["מין"];
                            if (portfolioitem.Gender == "ז")
                                portfolioitem.Gender = "1";
                            else
                                portfolioitem.Gender = "2";
                            
                            portfolioitem.ToshavIsrael = 1;
                            portfolioitem.ShnatMas = ctrlThis.shnatmas;
                            portfolioitem.IsNew = "0";
                            //portfolioitem.Name = portfolioitem["מחלקה"];
                          
                         
                            //
                            //portfolioitem.MatchType = portfolioitem["כתובת"];
                            //portfolioitem.CustomerSearchTerm = portfolioitem["עיר"];
                            //portfolioitem.Impressions = portfolioitem['דוא"ל'];
                            //portfolioitem.Clicks = portfolioitem["טלפון"];
                            //portfolioitem.CTR = portfolioitem["סוג משרה"];
                            //portfolioitem.CPC = portfolioitem["תאור סוג משרה"];
                            //portfolioitem.Spend = portfolioitem["מין"];
                            //portfolioitem.ACOS = portfolioitem["מצב משפחתי"];
                            //portfolioitem.Days7 = portfolioitem["בן זוג עובד"];
                            //portfolioitem.Days7 = portfolioitem["תאריך התחלה"];
                            //portfolioitem.Days7 = portfolioitem["תאריך סיום"];
                            
                          
                            //  portfolioitem.CurrentAsin = $.trim(portfolioitem.Campaign.slice(0, portfolioitem.Campaign.indexOf('-')));


                        });




                        ctrlThis.sendToServer(portfolioitems);



                    };
                    reader.readAsBinaryString(f);
                }
            }
            var input = $('<input type="file" style="display:none;">');
            $('body').append(input);
            $(input).on('change', handleFile);
            $(input).click();
        }

        function _sendToServer(portfolioitems) {

          
            if (portfolioitems.length == 0) {

                location.reload();

                //usersService.MWSService().then(function () {
                alert("העובדים נטענו...");

                //}.bind(this));



                return;

            }


            var ctrlThis = this;

            var Excellength = portfolioitems.length % 3000;

            var excelPost = portfolioitems.slice(ctrlThis.startIndex, Excellength);


            usersService.importWorkers(excelPost, ctrlThis.startIndex).then(function () {


                ctrlThis.startIndex = Excellength;
                excelPost = portfolioitems.slice(ctrlThis.startIndex + 1, portfolioitems.length);
                ctrlThis.sendToServer(excelPost);


            }.bind(this));

        }

        function _downloadData(report) {
           
            var thisCtrl = this;
            usersService.downloadAllManagerFiles(report.Id,this.shnatmas).then(function (data) {

                if (data == "1") {
                   
                    var url = thisCtrl.uploadsUri + thisCtrl.shnatmas + "/" + report.Id + ".zip";
                   // { { $ctrl.uploadsUri + worker.Id + '/OfekAllPdf.pdf' } }
                    window.open(url,"_blank");

                }
               
            });

        }

        function _getHebArea(id, num) {

            if (!id) return "";
            var res = this.sharedValues.areas.filter(x => x.id == id);
            if (res.length > 0) {

                res = this.sharedValues.areas.filter(x => x.id == id)[0].name;

                if (num) res = "," + res;
                return res;//(users[i].Role);
            }
            else
                return "";
        }

    }



})();
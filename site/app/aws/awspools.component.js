(function () {

    var app = angular.module('app');

    app.component('awspools', {
        templateUrl: 'app/aws/awspools.template.html',
        controller: AwspoolsController,
        bindings: {
            portfoliowords: '<',
            protname: '<',
            portfolioid: '<',
            portfolio: '<',
        }
    });

    function AwspoolsController(usersService, $scope, sharedValues, $http, $state) {


        this.role = localStorage.getItem('currentRole');

        var self = this;
        this.usersService = usersService;

        this.setStatusForAsin = _setStatusForAsin.bind(this);
        this.FilterAsins = _FilterAsins.bind(this);
        this.SaveAsins = _SaveAsins.bind(this);
        this.Summery = _Summery.bind(this);
        this.downloadExcel = _downloadExcel.bind(this);
        this.downloadExcelAmazone = _downloadExcelAmazone.bind(this);
        this.addToBulkMasterPhrase = _addToBulkMasterPhrase.bind(this);
        this.removeWords = _removeWords.bind(this);
        this.AddHard = _AddHard.bind(this);
        this.openGoogle = _openGoogle.bind(this);
        
        
        this.NextPrev = _NextPrev.bind(this);

        this.tempportfolioasins = this.portfolioasins;
        this.count = 2;
        function _openGoogle(portfolioword) {
            var payWind = window.open("https://www.google.com/search?tbm=isch&as_q=" + portfolioword.Word+"" , "Upload Chapter content", "width=1000,height=600,top=800,left=450");
          
        }


        function _removeWords(portfolioword) {
            //for (var i in this.portfoliowords) {
            //    if (this.portfoliowords[i] == portfolioword) {
            //        this.portfoliowords.splice(i, 1);
            //    }
            //}

            //מחיקה
            var thisCtrl = this;
            usersService.setWords(this.portfolioid, portfolioword.Word, portfolioword.Type, 2).then(function (res) {
              
                thisCtrl.portfoliowords = res;

            });
        }

        function _AddHard(type) {

            

            if (type==2 && this.newHardPositive) {
                var thisCtrl = this;
                usersService.setWords(this.portfolioid, this.newHardPositive,type,1).then(function (res) {
                   
                    thisCtrl.portfoliowords = res;

                });


            }


            if (type ==1 && this.newHardNegative) {
                var thisCtrl = this;
                usersService.setWords(this.portfolioid, this.newHardNegative, type, 1).then(function (res) {

                    thisCtrl.portfoliowords = res;

                });


            }
        }

        function _addToBulkMasterPhrase() {


            var positiveWord = this.portfoliowords.filter(x => x.Type == "1");

         
            usersService.getAllPortfliositemsForWords(this.portfolioid, this.count, positiveWord).then(function () {


                alert("The List Added to bulk master");
              //  thisCtrl.portfoliowords = res;
                //  $('#modal').modal('show');

            });
            //alert(positiveAsins.length);

            ////if (positiveAsins.length == 0) {
            ////    alert("No Positive Asins");
            ////    return;
            ////}

            //usersService.updatePortfoliosAsins(0, this.tempportfolioasins).then(function () {
            //    // alert("נשמר בהצלחה");
            //});
        }

        function _NextPrev(type) {
          

            for (var i in this.portfolio) {

                if (this.portfolio[i].Id == this.portfolioid) {


                    if (type == 1 && this.portfolio[eval(i) + 1]) {
                        $state.go('awspools', { id: this.portfolio[eval(i) + 1].Id, name: this.portfolio[eval(i) + 1].Name });
                        break;
                    }

                    if (type == 2 && i!=0) {
                        $state.go('awspools', { id: this.portfolio[eval(i) - 1].Id, name: this.portfolio[eval(i) - 1].Name });
                        break;
                    }


                }

            }



        }

        function _FilterAsins() {
            if (this.Asins == -1) {
                this.portfolioasins = this.tempportfolioasins;
            }
            if (this.Asins == 0) {
                this.portfolioasins = this.tempportfolioasins.filter(x => x.Status == 0 || x.Status == null);
            }
            if (this.Asins == 1) {
                this.portfolioasins = this.tempportfolioasins.filter(x => x.Status == 1);
            }



            //  alert(this.Asins);
        }

        function _SaveAsins() {


            // var positiveAsins = this.tempportfolioasins.filter(x => x.Status == 1);

            //if (positiveAsins.length == 0) {
            //    alert("No Positive Asins");
            //    return;
            //}

            //usersService.updatePortfoliosAsins(0, this.tempportfolioasins).then(function () {
            //    // alert("נשמר בהצלחה");
            //});
        }

        function _Summery() {


            var thisCtrl = this;

            usersService.getAllPortfliositemsForWords(this.portfolioid,this.count,null).then(function (res) {

              

                thisCtrl.portfoliowords = res;
              //  $('#modal').modal('show');

            });
        }

        function _setStatusForAsin(portfolio, val) {

            portfolio.Status = val;
        }

        function _downloadExcelAmazone() {
            var data = [];
            data.push([
                'Record ID',
                'Record Type',
                'Campaign ID',
                'Campaign',
                'Campaign Daily Budget',
                'Campaign Start Date',
                'Campaign End Date',
                'Campaign Targeting Type',
                'Portfolio ID',
                'Ad Group',
                'Max Bid',
                'Keyword or Product Targeting',
                'Product Targeting ID',
                'Match Type',
                'SKU',
                'Campaign Status',
                'Ad Group Status',
                'Status',
                'Bidding strategy',
                'Placement Type',
            ]);

            for (var portfolioword of this.portfoliowords) {
                if (portfolioword.Type == "2") continue;

                data.push([
                    '',
                    'Keyword',
                    '',
                    portfolioword.Campaign,
                    '',
                    '',
                    '',
                    '',
                    '',
                    '',
                    '',
                    portfolioword.Word,
                    '',
                    'campaign negative phrase',
                    '',
                    '',
                    '',
                    'enabled',
                    '',
                    '',

                ]);

            }


            _getReport(data);


        }

        function _downloadExcel() {
            var data = [];
            data.push([
                'Name',
                'Impressions',
                'CTR',
                'CPC',
                'Spend'

            ]);

            for (var portfolioitem of this.portfolioitems.filter(x => !x.Remove)) {
                data.push([
                    portfolioitem.CustomerSearchTerm,
                    portfolioitem.Impressions,
                    portfolioitem.CTR,
                    portfolioitem.CPC,
                    portfolioitem.Spend,

                ]);

            }


            _getReport(data);


        }

        function _getReport(rows) {
            function s2ab(s) {
                var buf = new ArrayBuffer(s.length);
                var view = new Uint8Array(buf);
                for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
                return buf;
            }

            var data = rows;

            var ws_name = "SheetJS";

            var wscols = [];

            /*console.log("Sheet Name: " + ws_name);
            console.log("Data: "); for (var i = 0; i != data.length; ++i) console.log(data[i]);
            console.log("Columns :"); for (i = 0; i != wscols.length; ++i) console.log(wscols[i]);*/

            /* dummy workbook constructor */
            function Workbook() {
                if (!(this instanceof Workbook)) return new Workbook();
                this.SheetNames = [];
                this.Sheets = {};
            }
            var wb = new Workbook();

            /* TODO: date1904 logic */
            function datenum(v, date1904) {
                if (date1904) v += 1462;
                var epoch = Date.parse(v);
                return (epoch - new Date(Date.UTC(1899, 11, 30))) / (24 * 60 * 60 * 1000);
            }
            /* convert an array of arrays in JS to a CSF spreadsheet */
            function sheet_from_array_of_arrays(data, opts) {
                var ws = {};
                var range = { s: { c: 10000000, r: 10000000 }, e: { c: 0, r: 0 } };
                for (var R = 0; R != data.length; ++R) {
                    for (var C = 0; C != data[R].length; ++C) {
                        if (range.s.r > R) range.s.r = R;
                        if (range.s.c > C) range.s.c = C;
                        if (range.e.r < R) range.e.r = R;
                        if (range.e.c < C) range.e.c = C;
                        var cell = { v: data[R][C] };
                        if (cell.v == null) continue;
                        var cell_ref = XLSX.utils.encode_cell({ c: C, r: R });

                        /* TEST: proper cell types and value handling */
                        if (typeof cell.v === 'number') cell.t = 'n';
                        else if (typeof cell.v === 'boolean') cell.t = 'b';
                        else if (cell.v instanceof Date) {
                            cell.t = 'n'; cell.z = XLSX.SSF._table[14];
                            cell.v = datenum(cell.v);
                        }
                        else cell.t = 's';
                        ws[cell_ref] = cell;
                    }
                }

                /* TEST: proper range */
                if (range.s.c < 10000000) ws['!ref'] = XLSX.utils.encode_range(range);
                return ws;
            }
            var ws = sheet_from_array_of_arrays(data);

            /* TEST: add worksheet to workbook */
            wb.SheetNames.push(ws_name);
            wb.Sheets[ws_name] = ws;

            /* TEST: column widths */
            ws['!cols'] = wscols;

            var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary', cellStyles: true });
            saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), "report.xlsx");
        }



    }




})();
(function () {

    var app = angular.module('app');

    app.component('awsauto', {
        templateUrl: 'app/aws/awsauto.template.html',
        controller: AwsautoController,
        bindings: {
            portfolioasins: '<',
            protname: '<',
            portfolio: '<',
            portfolioid:'<',
        }
    });

    function AwsautoController(usersService, $scope, sharedValues, $http, $state) {


        this.role = localStorage.getItem('currentRole');

        var self = this;
        this.usersService = usersService;

        this.setStatusForAsin = _setStatusForAsin.bind(this);
        this.FilterAsins = _FilterAsins.bind(this);
        this.SaveAsins = _SaveAsins.bind(this);
        this.Summery = _Summery.bind(this);
        this.downloadExcel = _downloadExcel.bind(this);
        this.downloadExcelAmazone = _downloadExcelAmazone.bind(this);
        this.NextPrev = _NextPrev.bind(this);
        this.removeNegative = _removeNegative.bind(this);

        this.addToBulkMaster = _addToBulkMaster.bind(this);
        this.isAsininCs = _isAsininCs.bind(this);
        
        this.tempportfolioasins = this.portfolioasins;
        this.count = 3;

        function _isAsininCs(CustomerSearchTerm) {
          
            if ((CustomerSearchTerm.toLowerCase()).startsWith("b0")) return true;

            return false;

          
        }

        function _addToBulkMaster() {

               
                
            usersService.updatePortfoliosItemsNegative(this.portfolioid, this.portfolioitems).then(function () {
                    alert("The List Added to bulk master");
             });
        }

        function _removeNegative(portfolioitem) {

           // portfolioitem.Remove = true;
            for (var i in this.portfolioitems) {
                if (this.portfolioitems[i] == portfolioitem) {
                    this.portfolioitems.splice(i, 1);
                }
            }

        }

        function _NextPrev(type) {
           
            this.portfolio = this.portfolio.filter(x => x.Status != 1);

            for (var i in this.portfolio) {



                if (this.portfolio[i].Name == this.protname) {


                    if (type == 1 && this.portfolio[eval(i) + 1] && this.portfolio[eval(i) + 1].Status == 1) {
                        alert("Not Allow Move to Portoflio if is in Proggress");
                        return;

                    }

                    if (type == 2 && this.portfolio[eval(i) - 1] && this.portfolio[eval(i) - 1].Status == 1) {
                        alert("Not Allow Move to Portoflio if is in Proggress");
                        return;

                    }

                  

                    if (type == 1 && this.portfolio[eval(i) + 1]) {
                        this.SaveAsins();
                        $state.go('awsauto', { id: this.portfolio[eval(i) + 1].Id, name: this.portfolio[eval(i) + 1].Name });
                        break;
                    }

                    if (type == 2 && i != 0) {
                        this.SaveAsins();
                        $state.go('awsauto', { id: this.portfolio[eval(i) - 1].Id, name: this.portfolio[eval(i) - 1].Name });
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

           
           // if (this.tempportfolioasins.length > 0)

            // var positiveAsins = this.tempportfolioasins.filter(x => x.Status == 1);

            //if (positiveAsins.length == 0) {
            //    alert("No Positive Asins");
            //    return;
            //}

            usersService.updatePortfoliosAsins(0, this.tempportfolioasins,this.portfolioid).then(function () {
                // alert("נשמר בהצלחה");
            });
        }

        function _Summery() {

           // this.SaveAsins();



            //if (this.tempportfolioasins.length == 0) {

            //    alert("The Asins detected");
            //    return;

            //}

            if (!this.count || this.count < 1) {
                alert("No Min Number Set!");
                return;
            }
            var thisCtrl = this;

        
            var newDocs = this.tempportfolioasins.map(function (x) {
                return {
                    Id: x.PortfolioAsinId,
                    Status: x.Status,
                    AsinId:x.AsinId,
                    PortfoliosId: x.PortfoliosId,
                    PortfolioItemsId: x.PortfolioItemsId
                };
            });

            usersService.updatePortfoliosAsins(0, newDocs, thisCtrl.portfolioid).then(function () {
                usersService.updatePortfoliosAsins(thisCtrl.count, newDocs, thisCtrl.portfolioid).then(function (res) {

                    thisCtrl.portfolioitems = res;
                    $('#modal').modal('show');

                });
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

            for (var portfolioitem of this.portfolioitems.filter(x => !x.Remove)) {
                portfolioitem.CustomerSearchTerm = portfolioitem.CustomerSearchTerm.replace(/[$#@!,.:^%&*)()/?<>"; ~]/g, ' ');
                var IsAsin = portfolioitem.CustomerSearchTerm.toLowerCase().startsWith("b0");

                data.push([
                    '',
                    (IsAsin) ? 'Product Targeting' : 'Keyword',
                    '',
                    portfolioitem.Campaign,
                    '',
                    '',
                    '',
                    '',
                    '',
                    (IsAsin) ? portfolioitem.GroupName : '',
                    '',
                    (IsAsin) ? ('asin="' + portfolioitem.CustomerSearchTerm + '"')  : portfolioitem.CustomerSearchTerm,
                    (IsAsin) ? ('asin="' + portfolioitem.CustomerSearchTerm + '"') : '',
                    (IsAsin) ? 'Negative Targeting Expression' : 'campaign negative exact',
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
(function () {

    var app = angular.module('app');

    app.component('awsmangershistory', {
        templateUrl: 'app/awshistory/awsmangershistory.template.html',
        controller: AwsmangerhistoryController,
        bindings: {
            portfolios: '<',

        }
    });

    function AwsmangerhistoryController(usersService, $scope, sharedValues, $http) {


        this.role = localStorage.getItem('currentRole');

        var self = this;
        this.usersService = usersService;
        this.upload = _upload.bind(this);
        this.removePortfolios = _removePortfolios.bind(this);
        this.getStatusName = _getStatusName.bind(this);
        this.sendToServer = _sendToServer.bind(this);
        this.downloafile = _downloafile.bind(this);
        this.GetHistoryData = _GetHistoryData.bind(this);
        this.SelectedRow = -1;
        this.SelectedRowItems = -1;
        this.checkAll = _checkAll.bind(this);
        this.setStatusForAsin = _setStatusForAsin.bind(this);

        this.downloadExcel = _downloadExcel.bind(this);
        this.downloadExcelAmazone = _downloadExcelAmazone.bind(this);
        this.filterData = _filterData.bind(this);
        this.setStatusForTerm = _setStatusForTerm.bind(this);
        

        this.count = 3;


        function _setStatusForTerm(portfolioitemId, changeType) {

            usersService.setStatusForTerm(portfolioitemId, changeType).then(function (res) {
             


            }.bind(this));
            
        }




        function _filterData(type) {

            if (type == 1) {

                try {
                    var portfolioitemsList = this.portfolioitems.filter(x => (this.filteritems == 1 && x.IsNegative == 1) || !this.filteritems
                        || this.filteritems == 0 || (this.filteritems == 2 && x.IsNegative != 1));

                    return portfolioitemsList.length;

                } catch {

                    return "0";
                }

            } else {
                try {
                    
                var portfolioasinsList = this.portfolioasins.filter(x => (this.filteritemsasin == 1 && x.Status != 1) || !this.filteritemsasin
                    || this.filteritemsasin == 0 || (this.filteritemsasin == 2 && x.Status == 1));


                    return portfolioasinsList.length;
                } catch {

                    return "0";
                }

            }

          

        }



        function _checkAll() {



            for (var i in this.portfolioitems) {
                this.portfolioitems[i].IsSelected = this.checkAllData;

            }

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

            for (var portfolioitem of this.portfolioitems.filter(x => x.IsSelected && x.IsNegative == 1)) {
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
                    (IsAsin) ? ('asin="' + portfolioitem.CustomerSearchTerm + '"') : portfolioitem.CustomerSearchTerm,
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

            for (var portfolioitem of this.portfolioitems.filter(x => x.IsSelected && x.IsNegative != 1)) {
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




        function _setStatusForAsin(portfolio, val) {

            portfolio.Status = val;

            usersService.SetHistoryData(this.count, portfolio).then(function (res) {
              //  this.portfolioitems = res
                //if (type == 1)
                //    this.portfolioitems = res;
                //if (type == 2)
                //    this.portfolioasins = res;


            }.bind(this));

        }


        function _GetHistoryData(type, Id, index) {

         
            if (type == 1) {
                this.SelectedRow = index;
            }
            if (type == 2) {
                this.SelectedRowItems = index;
            }




            usersService.GetHistoryData(type, Id).then(function (res) {
               
                if (type == 1)
                    this.portfolioitems = res;
                if (type == 2 || type == 3)
                    this.portfolioasins = res;
               

            }.bind(this));

        }



        function _getStatusName(statusId) {
            if (statusId == 1) return "Pending Download";
            if (statusId == 2) return "Ready to sort";
            if (statusId == 3) return "Finish";

        }

        function _removePortfolios(portfolio) {
            for (var i in this.portfolios) {
                if (this.portfolios[i] == portfolio) {
                    this.portfolios.splice(i, 1);
                }
            }


            usersService.updatePortfolios(this.portfolios).then(function () {
                alert("נשמר בהצלחה");
            });
        }

        function _upload() {


            if (this.portfolios.length > 0) {

                var currentportfolios = this.portfolios.filter(x => x.Status != 3 && x.RowCount != 0);
                if (currentportfolios.length > 0) {

                    alert("You Must Finish Work all Current Portfolios Before Upload New Excel");
                    return;
                }

            }

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

                        var portfolioitems = XLSX.utils.sheet_to_json(workbook.Sheets[workbook.SheetNames[0]]);





                        portfolioitems.map(function (portfolioitem) {

                            portfolioitem.Id = 0;
                            portfolioitem.StartDate = portfolioitem["Start Date"];
                            portfolioitem.EndDate = portfolioitem["End Date"];
                            portfolioitem.Name = portfolioitem["Portfolio name"];
                            portfolioitem.Currency = portfolioitem["Currency"];
                            portfolioitem.Campaign = portfolioitem["Campaign Name"];
                            portfolioitem.GroupName = portfolioitem["Ad Group Name"];
                            portfolioitem.MatchType = portfolioitem["Match Type"];
                            portfolioitem.CustomerSearchTerm = portfolioitem["Customer Search Term"];
                            portfolioitem.Impressions = portfolioitem["Impressions"];
                            portfolioitem.Clicks = portfolioitem["Clicks"];
                            portfolioitem.CTR = portfolioitem["Click-Thru Rate (CTR)"];
                            portfolioitem.CPC = portfolioitem["Cost Per Click (CPC)"];
                            portfolioitem.Spend = portfolioitem["Spend"];
                            portfolioitem.ACOS = portfolioitem["Total Advertising Cost of Sales (ACoS) "];
                            portfolioitem.Days7 = portfolioitem["7 Day Total Sales "];

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

        this.startIndex = 0;

        function _sendToServer(portfolioitems) {

            if (portfolioitems.length == 0) {

                location.reload();
                return;
            }


            var ctrlThis = this;

            var Excellength = portfolioitems.length % 3000;

            var excelPost = portfolioitems.slice(ctrlThis.startIndex, Excellength);


            usersService.importPortfolios(excelPost, ctrlThis.startIndex).then(function () {


                ctrlThis.startIndex = Excellength;
                excelPost = portfolioitems.slice(ctrlThis.startIndex + 1, portfolioitems.length);
                ctrlThis.sendToServer(excelPost);


            }.bind(this));

        }

        function _downloafile(type) {
            var data = [];

            data.push([
                'ASIN',
                'Price'

            ]);

            //NEGATIVE
            if (type == 1) {

               
              

                for (var portfolioasin of this.portfolioasins.filter(x => x.Status != 1)) {
                    data.push([
                        portfolioasin.Asin,
                        portfolioasin.Price
                    ]);

                }

               


            }
             //POSITIVE
            if (type == 2) {

                for (var portfolioasin of this.portfolioasins.filter(x => x.Status == 1)) {
                    data.push([
                        portfolioasin.Asin
                    ]);

                }

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
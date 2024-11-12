(function () {

    var app = angular.module('app');

  
    app.component('workers', {
        templateUrl: 'app/workers/workers.template.html?v=2',
        controller: WorkersController,
        bindings: {
            workers: '<'
            
        }
    });

    function WorkersController(usersService, sharedValues, $state) {

   
        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);
        this.checkAll = _checkAll.bind(this);
        this.deleteAll = _deleteAll.bind(this);
        

        this.delete = _delete.bind(this);
        this.downloadExcel = _downloadExcel.bind(this);
        this.uploadsUri = sharedValues.apiUrl + '/uploads/';



        function _downloadExcel() {

            var data = [];
            data.push([
                
                'ת.ז.',
                'מין',
                'שם פרטי',
                'שם משפחה',
                'טלפון',
                'טלפון נייד',
                'כתובת',
                'מייל',
                'חבר קופת חולים',
                'קופת חולים',

                'שכר שעתי',
                'שכר חודשי',
                'מצב משפחתי',
                'תחילת עבודה',
                'תאריך לידה',
                'עיר',
                'סוג משרה',
                'בנק',
                'סניף',
                'חשבון'


              /*  שכר שעתי, שכר חודשי, נסיעות, מספר עובד, מצב משפחתי, מספר הנהלח, תחילת עבודה, תאריך לידה, עיר, סוג משרה, דרוג, דרגה, תת מפעל, בנק, סניף, חשבון*/



            ]);

            this.workers.forEach(function (worker) {
                data.push([
                    worker.Taz,
                    (worker.Gender == 1) ? "זכר" : "נקבה",
                    worker.FirstName,
                    worker.LastName,
                    worker.Phone,
                    worker.PhoneSelular,
                    worker.FullAddress,
                    worker.Email,
                    (worker.KupatHolim == 1) ? "כן" : "לא",
                    getKupaName(worker.Kupa),
                    worker.HeskemWorkerMaskuert3,
                    worker.HeskemWorkerMaskuert4,
                    getMatzavMish(worker.StatusFamely),
                    getFormatDate(worker.StartWorkDate),
                    getFormatDate(worker.BirthDate),
                    worker.City,
                    getSugMisra(worker.SugMaskuret),
                    worker.BankNumName,
                    worker.BrunchNumName,
                    worker.BankAccountNumber


                ]);
            });

            _getReport(data);


            function getFormatDate(date) {
                if (date)
                    return moment(date).format("DD-MM-YYYY");
                else
                    return "";
                
            }

            function getSugMisra(SugMaskuret) {

                switch (SugMaskuret) {
                    case '1':
                        return "משכורת חודש";
                        break;
                    case '2':
                        return "שכר עבודה (עובד יומי)";
                        break;
                    case '3':
                        return "משכורת נוספת";
                        break;
                    case '4':
                        return "קצבה";
                        break;
                    case '5':
                        return "משכורת חלקית";
                        break;

                    case '6':
                        return "מלגה";
                        break;
                    default:
                        return "";
                }




            }

            function getMatzavMish(StatusFamely) {

                switch (StatusFamely) {
                    case '1':
                        return "רווק/ה";
                        break;
                    case '2':
                        return "נשוי/אה";
                        break;
                    case '3':
                        return "גרוש/ה";
                        break;
                    case '4':
                        return "אלמן/ה";
                        break;
                    case '5':
                        return "פרוד/ה";
                        break;
                    default:
                        return "";
                }


            }

            function getKupaName(Kupa) {

                switch (Kupa) {
                    case '1':
                        return "כללית";
                        break;
                    case '2':
                        return "מכבי";
                        break;
                    case '3':
                        return "מאוחדת";
                        break;
                    case '4':
                        return "לאומית";
                        break;

                    default:
                        return "";
                }
                

            }

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

        function _checkAll() {

         
            this.workers.forEach(x => x.IsSelected = this.checkAllc);
        }




        function _getHebRole(id) {

        
            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }

        function _deleteAll() {

            if (confirm('האם למחוק את כל העובדים המסומנים?')) {


                var selected = this.workers.filter(x => x.IsSelected);

                for (var i in selected) {
                    var ctrl = this;
                    usersService.deleteWorker(selected[i].Id, true).then(function (res) {


                         ctrl.workers = res;
                       
                    });

                }


            }

        }
        function _delete(workerid) {
            if (confirm('האם למחוק את העובד?')) {
              
                var ctrl = this;
                usersService.deleteWorker(workerid,true).then(function (res) {

                   
                    ctrl.workers = res;
                   // $state.go('workers');
                    //ctrl.user.Deleted = true;
                    //$state.go('students');
                });
            }
        }
    }

})();
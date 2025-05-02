using FarmsApi.DataModels;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Results;
using System.Xml;

namespace FarmsApi.Services
{
    public class FarmsService
    {
        public static List<Farm> GetFarms(bool deleted)
        {
            using (var Context = new Context())
            {
                var CurrentUser = UsersService.GetCurrentUser();
                if (CurrentUser.Role == "sysAdmin")
                    return Context.Farms.Where(f => f.Deleted == deleted).ToList();
                else if (CurrentUser.Role == "vetrinar" || CurrentUser.Role == "shoeing")
                    return GetFarmsByRole(Context, CurrentUser);//Context.Farms.Where(f => f.Deleted == deleted).ToList();
                else
                    return Context.Farms.Where(f => f.Deleted == deleted && f.Id == CurrentUser.Farm_Id).ToList();
            }
        }

        private static List<Farm> GetFarmsByRole(Context context, User currentUser)
        {

            var FarmList = context.FarmManagers.Where(x => x.MefarzelUser == currentUser.Email || x.VetrinarUser == currentUser.Email).ToList();

            var Farms = context.Farms.ToList();
            var ReturnFarms = Farms.Where(y => y.Id == currentUser.Farm_Id || FarmList.Any(f => f.FarmId == y.Id)).ToList();

            return ReturnFarms;


        }

        public static User GetFarmsMainUser(int FarmId)
        {
            using (var Context = new Context())
            {

                return Context.Users.Where(x => x.Farm_Id == FarmId && (x.Role == "farmAdmin" || x.Role == "farmAdminHorse" || x.Role == "vetrinar" || x.Role == "shoeing")).FirstOrDefault();


            }
        }



        public static void DeleteFarm(int id)
        {
            using (var Context = new Context())
            {
                var Farm = Context.Farms.SingleOrDefault(f => f.Id == id);
                Farm.Deleted = true;
                foreach (var User in Context.Users.Where(u => u.Farm_Id == id))
                {
                    User.Deleted = true;
                }
                //foreach (var Horse in Context.Horses.Where(h => h.Farm_Id == id))
                //{
                //    Horse.Deleted = true;
                //}
                //Context.Notifications.RemoveRange(Context.Notifications.Where(n => n.FarmId == id));

                Context.SaveChanges();
            }
        }



       

        public static Farm GetFarm(int Id)
        {
            using (var Context = new Context())
            {

                Farm farm = Context.Farms.SingleOrDefault(u => u.Id == Id);


                if (farm != null)
                {

                    var root = HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + Id.ToString());

                    


                    if (Directory.Exists(root + "/Logo/"))
                    {
                        string[] files = System.IO.Directory.GetFiles(root + "/Logo/");
                        if (files.Length != 0)
                        {

                            farm.Logo = Path.GetFileName(files[0]);
                        }
                    }
                    if (File.Exists(root + "/PDFS/SignatureAmuta.png"))
                    {
                        

                            farm.Sign = "SignatureAmuta.png";
                        
                    }
                }

                return farm;

            }

        }

        public static FarmManagers GetMangerFarm()
        {
            using (var Context = new Context())
            {
                var CurrentUser = UsersService.GetCurrentUser();

                return Context.FarmManagers.SingleOrDefault(u => u.FarmId == CurrentUser.Farm_Id);

            }

        }

        public static IEnumerable<FarmInstructors> GetMangerInstructorFarm()
        {
            using (var Context = new Context())
            {
                var CurrentUser = UsersService.GetCurrentUser();
                var CurrentUserFarmId = CurrentUser.Farm_Id;

                var InstructorList = from u in Context.Users
                                     from um in Context.FarmInstructors.Where(x => x.UserId == u.Id).DefaultIfEmpty()//  on u.Id equals um.UserId
                                     where (u.Role == "instructor" || u.Role == "profAdmin") && u.Farm_Id == CurrentUserFarmId && !u.Deleted
                                     select new { Id = u.Id, ClalitNumber = um.ClalitNumber, UserId = u.Id, InstructorName = u.FirstName + " " + u.LastName };


                var l = InstructorList.ToList().Select(x => new FarmInstructors { Id = x.Id, ClalitNumber = x.ClalitNumber, UserId = x.UserId, InstructorName = x.InstructorName });

                return l;

            }

        }

        public static FarmManagers SetMangerFarm(FarmManagers farmmanger)
        {

            var CurrentUser = UsersService.GetCurrentUser();
            using (var Context = new Context())
            {

                if (farmmanger.Id == 0)
                {
                    farmmanger.FarmId = CurrentUser.Farm_Id;
                    Context.FarmManagers.Add(farmmanger);


                }
                else
                {
                    Context.Entry(farmmanger).State = System.Data.Entity.EntityState.Modified;

                }


                Context.SaveChanges();

                return Context.FarmManagers.Where(x => x.FarmId == CurrentUser.Farm_Id).FirstOrDefault();

            }




            //return true;

        }

        public static bool SetMangerInstructorFarm(List<FarmInstructors> farmInstructors)
        {

            var CurrentUser = UsersService.GetCurrentUser();
            using (var Context = new Context())
            {



                foreach (FarmInstructors item in farmInstructors)
                {

                    FarmInstructors dbfarmInstructor = Context.FarmInstructors.Where(x => x.UserId == item.UserId).FirstOrDefault();

                    if (dbfarmInstructor != null)
                    {
                        dbfarmInstructor.ClalitNumber = item.ClalitNumber;

                        Context.Entry(dbfarmInstructor).State = System.Data.Entity.EntityState.Modified;
                        //Context.SaveChanges();

                    }
                    else
                    {
                        Context.FarmInstructors.Add(item);

                    }


                }
                Context.SaveChanges();


                //Context.Entry(farmmanger).State = System.Data.Entity.EntityState.Modified;
                //Context.SaveChanges();

            }

            return true;

        }

        public static Farm UpdateFarm(Farm Farm)
        {
            using (var Context = new Context())
            {
                if (Farm.Id == 0)
                    Context.Farms.Add(Farm);
                else
                    Context.Entry(Farm).State = System.Data.Entity.EntityState.Modified;

                Context.SaveChanges();
                return Farm;
            }
        }





        //public static List<KlalitHistoris> GetKlalitHistoris(int FarmId, string startDate = null, string endDate = null, int? type = null, int? klalitId = null)
        //{
        //    using (var Context = new Context())
        //    {


        //        if (type == null) type = -1;
        //        if (klalitId == null) klalitId = 0;


        //        SqlParameter FarmIdPara = new SqlParameter("FarmId", FarmId);
        //        SqlParameter StartDatePara = new SqlParameter("StartDate", startDate);
        //        SqlParameter EndDatePara = new SqlParameter("EndDate", endDate);
        //        SqlParameter TypePara = new SqlParameter("Type", type);
        //        SqlParameter KlalitIdPara = new SqlParameter("KlalitId", klalitId);

        //        var query = Context.Database.SqlQuery<KlalitHistoris>
        //        ("GetKlalitHistoris  @FarmId,@StartDate,@EndDate,@Type,@KlalitId", FarmIdPara, StartDatePara, EndDatePara, TypePara, KlalitIdPara);
        //        var Objects = query.ToList();

        //        if ((type == 2 || type == 1) && Objects.Count > 0 && Objects[0].Id != -1)
        //        {


        //            int res = SendToKlalitAPI(FarmId, Objects, Context);
        //            if (res < 0)
        //            {
        //                return GetError(res);
        //            }

        //            return GetKlalitHistoris(FarmId, startDate, endDate, -1, 0);

        //        }

        //        return Objects;

        //    }
        //}

        //private static List<KlalitHistoris> GetError(int res)
        //{

        //    List<KlalitHistoris> kllist = new List<KlalitHistoris>();

        //    KlalitHistoris kh = new KlalitHistoris();
        //    kh.Id = res;
        //    kh.UserId = 0;
        //    kh.FarmId = 0;
        //    kh.DateSend = DateTime.Now;
        //    kh.DateLesson = DateTime.Now;

        //    kllist.Add(kh);
        //    return kllist;




        //}



        //public static KlalitHistoris SetKlalitHistoris(KlalitHistoris khv)
        //{



        //    using (var Context = new Context())
        //    {
        //        var item = khv;
        //        FarmManagers fm = Context.FarmManagers.Where(x => x.FarmId == item.FarmId).FirstOrDefault();
        //        IEnumerable<FarmInstructors> fi = GetMangerInstructorFarm();

        //        FarmInstructors fs = fi.Where(y => y.UserId == item.Instructor_Id).FirstOrDefault();


        //        if (fs == null)
        //        {
        //            if (item.KlalitHistorisId == null)
        //            {
        //                khv = new KlalitHistoris();
        //                khv.Id = 0;
        //                khv.UserId = item.UserId;
        //                khv.FirstName = khv.FirstName;
        //                khv.LastName = khv.LastName;
        //                khv.FarmId = item.FarmId;
        //                khv.DateSend = DateTime.Now;
        //                khv.DateLesson = item.DateLesson;
        //                khv.ClaimNumber = "";
        //                khv.ResultXML = "";
        //                khv.ResultNumber = "2";
        //                khv.Result = "לא נמצא מדריך המוגדר בכללית המתאים לבקשה";


        //                khv.CounterSend = 1;
        //                khv.Instructor_Id = item.Instructor_Id;
        //                // Context.KlalitHistoris.Add(khv);

        //            }
        //            else
        //            {
        //                khv.DateSend = DateTime.Now;
        //                khv.ResultXML = "";
        //                khv.ResultNumber = "2";
        //                khv.Result = "לא נמצא מדריך המוגדר בכללית המתאים לבקשה";
        //                khv.CounterSend += 1;
        //                Context.Entry(khv).State = System.Data.Entity.EntityState.Modified;
        //            }
        //            Context.SaveChanges();
        //            return khv;

        //        }

        //        string FirstName = item.FirstName;
        //        string LastName = item.LastName;
        //        string DateStart = item.DateLesson.ToString("ddMMyyyy");

        //        // KlalitAPI.SupplierRequest kp = new KlalitAPI.SupplierRequest();


        //        // kp.Url = "https://sapaktest.clalit.co.il/mushlamsupplierservice/SupplierRequest.asmx";

        //        string xml = @"
        //                            <XMLInput>
	       //                             <ActionCode>11</ActionCode>
	       //                             <UserName>" + fm.UserName + @"</UserName>
	       //                             <Password>" + fm.Password + @"</Password>
	       //                             <SupplierID>" + fm.SupplierID + @"</SupplierID>
	       //                             <ClinicID>0</ClinicID>
	       //                             <InsuredID>" + item.Taz + @"</InsuredID>
	       //                             <InsuredFirstName>" + FirstName.Trim() + @"</InsuredFirstName>
	       //                             <InsuredLastName>" + LastName.Trim() + @"</InsuredLastName>
	       //                             <SectionCode>" + fm.SectionCode + @"</SectionCode>
	       //                             <CareCode>" + fm.CareCode + @"</CareCode>
	       //                             <CareDate>" + DateStart + @"</CareDate>
	       //                             <DoctorID>" + fs.ClalitNumber + @"</DoctorID>
	       //                             <OnlineServiceType>0</OnlineServiceType>
        //                            </XMLInput>";
        //        //  var resXML = kp.SendXML(xml); //203700003 //203700007



        //        XmlDocument XmlRes = new XmlDocument();
        //        //XmlRes.LoadXml(resXML.ToString());

        //        var Result = XmlRes.DocumentElement.SelectSingleNode("Result").InnerText;// 1 נקלטה
        //        var AnswerDetails = XmlRes.DocumentElement.SelectSingleNode("AnswerDetails");// התשובה כאן
        //        var ErrorDescription = XmlRes.DocumentElement.SelectSingleNode("ErrorDescription");// השגיאה כאן
        //        var ClaimNumber = XmlRes.DocumentElement.SelectSingleNode("ClaimNumber");// מספר תביעה

        //        KlalitHistoris kh = item;

        //        if (item.KlalitHistorisId == null)
        //        {
        //            kh = new KlalitHistoris();
        //            kh.Id = 0;
        //            kh.UserId = item.UserId;
        //            kh.FirstName = khv.FirstName;
        //            kh.LastName = khv.LastName;
        //            kh.FarmId = item.FarmId;
        //            kh.DateSend = DateTime.Now;
        //            kh.DateLesson = item.DateLesson;
        //            kh.ClaimNumber = (ClaimNumber == null) ? "" : ClaimNumber.InnerText;
        //            //  kh.ResultXML = resXML.ToString();
        //            kh.ResultNumber = Result;
        //            kh.Result = (Int32.Parse(Result) > 0) ? AnswerDetails.InnerText : ErrorDescription.InnerText;

        //            if (Result == "1" && string.IsNullOrEmpty(kh.Result)) kh.Result = "קיימת כבר תביעה לתאריך";

        //            kh.CounterSend = 1;
        //            kh.Instructor_Id = item.Instructor_Id;
        //            //  Context.KlalitHistoris.Add(kh);

        //        }
        //        else
        //        {
        //            kh.DateSend = DateTime.Now;
        //            kh.DateLesson = item.DateLesson;
        //            kh.ClaimNumber = (ClaimNumber == null) ? "" : ClaimNumber.InnerText;
        //            // kh.ResultXML = resXML.ToString();
        //            kh.ResultNumber = Result;
        //            kh.Result = (Int32.Parse(Result) > 0) ? AnswerDetails.InnerText : ErrorDescription.InnerText;

        //            if (Result == "1" && string.IsNullOrEmpty(kh.Result)) kh.Result = "קיימת כבר תביעה לתאריך";

        //            kh.CounterSend += 1;
        //            kh.Instructor_Id = item.Instructor_Id;
        //            Context.Entry(kh).State = System.Data.Entity.EntityState.Modified;



        //        }

        //        Context.SaveChanges();
        //        return kh;
        //    }

        //}

        //private static int SendToKlalitAPI(int FarmId, List<KlalitHistoris> Objects, Context Context)
        //{
        //    int res = 0;
        //    FarmManagers fm = Context.FarmManagers.Where(x => x.FarmId == FarmId).FirstOrDefault();
        //    IEnumerable<FarmInstructors> fi = GetMangerInstructorFarm();
        //    if (fm != null)
        //    {
        //        // אין הגדרות למדריכים
        //        if (fi.Count() == 0) return -1;

        //        foreach (var item in Objects)
        //        {
        //            FarmInstructors fs = fi.Where(y => y.UserId == item.Instructor_Id).FirstOrDefault();

        //            if (fs == null) continue;


        //            string FirstName = item.FirstName;
        //            string LastName = item.LastName;
        //            string DateStart = item.DateLesson.ToString("ddMMyyyy");

        //            //KlalitAPI.SupplierRequest kp = new KlalitAPI.SupplierRequest();

        //            string xml = @"
        //                            <XMLInput>
	       //                             <ActionCode>11</ActionCode>
	       //                             <UserName>" + fm.UserName + @"</UserName>
	       //                             <Password>" + fm.Password + @"</Password>
	       //                             <SupplierID>" + fm.SupplierID + @"</SupplierID>
	       //                             <ClinicID>0</ClinicID>
	       //                             <InsuredID>" + item.Taz + @"</InsuredID>
	       //                             <InsuredFirstName>" + FirstName.Trim() + @"</InsuredFirstName>
	       //                             <InsuredLastName>" + LastName.Trim() + @"</InsuredLastName>
	       //                             <SectionCode>" + fm.SectionCode + @"</SectionCode>
	       //                             <CareCode>" + fm.CareCode + @"</CareCode>
	       //                             <CareDate>" + DateStart + @"</CareDate>
	       //                             <DoctorID>" + fs.ClalitNumber + @"</DoctorID>
	       //                             <OnlineServiceType>0</OnlineServiceType>
        //                            </XMLInput>";
        //            //   var resXML = kp.SendXML(xml); //203700003 //203700007

        //            XmlDocument XmlRes = new XmlDocument();
        //            //  XmlRes.LoadXml(resXML.ToString());

        //            var Result = XmlRes.DocumentElement.SelectSingleNode("Result").InnerText;// 1 נקלטה
        //            var AnswerDetails = XmlRes.DocumentElement.SelectSingleNode("AnswerDetails");// התשובה כאן
        //            var ErrorDescription = XmlRes.DocumentElement.SelectSingleNode("ErrorDescription");// השגיאה כאן
        //            var ClaimNumber = XmlRes.DocumentElement.SelectSingleNode("ClaimNumber");// מספר תביעה


        //            KlalitHistoris kh = Objects[0];

        //            kh.DateSend = DateTime.Now;
        //            kh.DateLesson = item.DateLesson;
        //            kh.ClaimNumber = (ClaimNumber == null) ? "" : ClaimNumber.InnerText;
        //            //  kh.ResultXML = resXML.ToString();
        //            kh.ResultNumber = Result;
        //            kh.Result = (Int32.Parse(Result) > 0) ? AnswerDetails.InnerText : ErrorDescription.InnerText;

        //            if (Result == "1" && string.IsNullOrEmpty(kh.Result)) kh.Result = "קיימת כבר תביעה לתאריך";

        //            kh.CounterSend += 1;
        //            kh.Instructor_Id = item.Instructor_Id;
        //            Context.Entry(kh).State = System.Data.Entity.EntityState.Modified;
        //            //KlalitHistoris kh = new KlalitHistoris();
        //            //kh.Id = 0;
        //            //kh.UserId = item.UserId;
        //            //kh.FarmId = item.FarmId;
        //            //kh.DateSend = DateTime.Now;
        //            //kh.DateLesson = item.DateLesson;
        //            //kh.ClaimNumber = (ClaimNumber == null) ? "" : ClaimNumber.InnerText;
        //            //kh.ResultXML = resXML.ToString();
        //            //kh.ResultNumber = Result;
        //            //kh.Result = (Int32.Parse(Result) > 0) ? AnswerDetails.InnerText : ErrorDescription.InnerText;

        //            //if (kh.Result.Contains("קיימת פניה דומה")) kh.Result = "קיימת כבר תביעה לתאריך";


        //            //kh.Instructor_Id = item.Instructor_Id;
        //            //Context.KlalitHistoris.Add(kh);

        //        }

        //        Context.SaveChanges();
        //    }
        //    else
        //    {
        //        // אין הגדרות לחווה שם משתמש
        //        return -2;


        //    }



        //    return res;
        //}
    }
}
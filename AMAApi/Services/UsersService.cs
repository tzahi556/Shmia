﻿using FarmsApi.DataModels;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.UI.WebControls;
using System.IO.Compression;
using System.Diagnostics;
using RestSharp;
using FarmsApi.Migrations;
using System.Security.Cryptography;
using System.Text;
using ThirdParty.Json.LitJson;
using System.Text.Json;
using Newtonsoft.Json;
using System.Web.Http.Results;
using Org.BouncyCastle.Utilities.Collections;

namespace FarmsApi.Services
{
    public class UsersService
    {


        private static double? CheckifExistDouble(JToken jToken)
        {
            if (jToken == null) return 0;
            double res;
            bool Ok = Double.TryParse(jToken.ToString(), out res);
            if (Ok) return res;
            else return 0;
        }

        private static bool CheckifExistBool(JToken jToken)
        {
            if (jToken == null) return false;
            Boolean res = new Boolean();
            bool Ok = Boolean.TryParse(jToken.ToString(), out res);
            if (Ok)
                return res;
            else
                return false;
        }

        private static DateTime? CheckifExistDate(JToken jToken)
        {

            if (jToken == null) return null;
            DateTime res = new DateTime();
            bool Ok = DateTime.TryParse(jToken.ToString(), out res);
            if (Ok && res.Year > 1960) return res;

            else return null;
        }

        private static int CheckifExistInt(JToken jToken)
        {
            if (jToken == null) return 0;
            int res;
            bool Ok = Int32.TryParse(jToken.ToString(), out res);
            if (Ok) return res;
            else return 0;
        }

        private static string CheckifExistStr(JToken jToken)
        {

            if (jToken != null)

                return jToken.ToString();

            return "";

        }




        //*****************************************************************************

        public static List<User> GetUsers(string Role, bool IncludeDeleted = false)
        {
            using (var Context = new Context())
            {
                var CurrentUserFarmId = GetCurrentUser().Farm_Id;



                Context.Configuration.ProxyCreationEnabled = false;
                Context.Configuration.LazyLoadingEnabled = false;

                var Users = Context.Users.Where(u => u.Farm_Id == CurrentUserFarmId).OrderBy(x => x.FirstName).ToList();

                if (CurrentUserFarmId == 0)
                {
                    Users = Context.Users.ToList();
                }

                Users = FilterByUser(Users);
                Users = FilterRole(Users, Role);
                Users = FilterDeleted(Users, IncludeDeleted);

                return RemovePassword(Users);
            }
        }

        private static List<User> FilterByUser(List<User> Users)
        {
            var CurrentUser = GetCurrentUser();

            if (CurrentUser.Role == "instructor" || CurrentUser.Role == "profAdmin")
                return Users.Where(u => u.Role == "student" || u.Id == CurrentUser.Id).ToList();

            return Users;
        }

        private static List<User> FilterByFarm(List<User> Users)
        {
            var CurrentUserFarmId = GetCurrentUser().Farm_Id;
            if (CurrentUserFarmId != 0)
                return Users.Where(u => u.Farm_Id == CurrentUserFarmId).ToList();
            else
                return Users;
        }

        private static List<User> FilterDeleted(List<User> Users, bool IncludeDeleted)
        {
            if (IncludeDeleted)
            {
                return Users;
            }

            return Users.Where(u => !u.Deleted).ToList();
        }

        private static List<User> FilterRole(List<User> Users, string Role)
        {
            if (!string.IsNullOrEmpty(Role))
            {
                var Roles = Role.Split(',');
                if (Roles.Length > 1)
                {
                    List<User> ReturnUsers = new List<User>();
                    foreach (var role in Roles)
                    {
                        ReturnUsers.AddRange(Users.Where(u => u.Role == role).ToList());
                    }
                    return ReturnUsers;
                }
                else
                {
                    return Users.Where(u => u.Role == Role).ToList();
                }
            }
            return Users;
        }

        public static User GetUser(int? Id)
        {
            using (var Context = new Context())
            {
                if (Id.HasValue)
                {
                    return Context.Users.SingleOrDefault(u => u.Id == Id.Value && !u.Deleted);
                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    return Context.Users.SingleOrDefault(u => u.Id == CurrentUserId);
                }

            }
        }

        public static User GetSetUserEnter(int? Id, bool isForCartis = false)
        {


            using (var Context = new Context())
            {
                if (Id.HasValue)
                {
                    User us = Context.Users.SingleOrDefault(u => u.Id == Id.Value && !u.Deleted);
                    if (isForCartis)
                    {

                        if (us != null) //&& us.CurrentUserId != GetCurrentUser().Id)
                        {

                            //var users = Context.Users.Where(x => x.CurrentUserId == user.Id).ToList();

                            //users.ForEach(a =>
                            //{
                            //    a.IsTafus = false;
                            //    a.CurrentUserId = null;
                            //});



                            //Context.SaveChanges();

                            //// אם לא תפוס  תתפוס
                            //if (!us.IsTafus)
                            //{
                            //    User u = GetCurrentUser();
                            //    us.IsTafus = true;
                            //    us.CurrentUserId = u.Id;
                            //    us.TofesName = u.FirstName + " " + u.LastName;
                            //    Context.Entry(us).State = System.Data.Entity.EntityState.Modified;
                            //    Context.SaveChanges();

                            //    us.IsTafus = false;

                            //}
                            //else
                            //{
                            //    // אם תפוס אבל אותו משתמש תחזיר שלא תפוס
                            //    if (us.CurrentUserId == GetCurrentUser().Id)
                            //        us.IsTafus = false;

                            //}

                        }


                    }
                    return us;
                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    return Context.Users.SingleOrDefault(u => u.Id == CurrentUserId);
                }

            }
        }

        public static List<Testpdfs> GetPortfolios(int llx, int lly, int urx, int ury, string text, int font, int space, int id, int pagenumber)
        {


            using (var Context = new Context())
            {

                if (id == 0)
                {
                    var PdfScena = new Testpdfs();//Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.llx = llx;
                    PdfScena.lly = lly;
                    PdfScena.urx = urx;
                    PdfScena.ury = ury;
                    PdfScena.Word = text;
                    PdfScena.Font = font;
                    PdfScena.Space = space;
                    PdfScena.PageNumber = pagenumber;

                    Context.Testpdfs.Add(PdfScena);
                    Context.SaveChanges();

                }


                if (id > 0)
                {
                    var PdfScena = Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.llx = llx;
                    PdfScena.lly = lly;
                    PdfScena.urx = urx;
                    PdfScena.ury = ury;
                    PdfScena.Word = text;
                    PdfScena.Font = font;
                    PdfScena.Space = space;
                    PdfScena.PageNumber = pagenumber;

                    Context.Entry(PdfScena).State = System.Data.Entity.EntityState.Modified;
                    Context.SaveChanges();


                }

                PdfAPI pa = new PdfAPI();
                pa.TestPdfNewFromDB(llx, lly, urx, ury, text);



                //כאשר אני משתמש על הpdf 
                //var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderByDescending(x => x.Id).ToList();

                //return TestList;

                // כאשר אני עושה התאמה אז אני משתמש כאן

                var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderBy(x => x.Id).ToList();

                return TestList;

            }


        }

        public static List<Testpdfs> BindData(int id, string Comment, int pagenumber, string Value)
        {


            using (var Context = new Context())
            {


                if (Comment == "null") Comment = null;
                if (Value == "null") Value = null;

                if (id > 0)
                {
                    var PdfScena = Context.Testpdfs.Where(x => x.Id == id).FirstOrDefault();
                    PdfScena.Comment = Comment;
                    PdfScena.Value = Value;

                    Context.Entry(PdfScena).State = System.Data.Entity.EntityState.Modified;
                    Context.SaveChanges();


                }




                var TestList = Context.Testpdfs.Where(x => x.PageNumber == pagenumber).OrderBy(x => x.Id).ToList();

                return TestList;

            }


        }


        //****************************************** Workers
        public static List<Files> GetFiles(int Workerid)
        {


            using (var Context = new Context())
            {


                var WorkersFilesList = Context.Files.Where(x => x.WorkerId == Workerid).ToList();

                return WorkersFilesList;


            }
        }

        public static List<WorkersWith101> GetWorkers(bool isnew)
        {


            using (var Context = new Context())
            {

                //if (id >= 0)
                //{

                //    var WorkersList = Context.Workers.Where(x => x.Id == id).ToList();
                //    return WorkersList;

                //}

                var CurrentUser = GetCurrentUser();

                var CurrentUserId = CurrentUser.Id;
                var CurrentRole = CurrentUser.Role;
                var CurrentFarmId = CurrentUser.Farm_Id;


                if (CurrentRole == "instructor")
                {
                    //var WorkersListToRemove = Context.Workers.Where(x => (string.IsNullOrEmpty(x.FirstName) && string.IsNullOrEmpty(x.LastName) && string.IsNullOrEmpty(x.Taz))).ToList();

                    ////  Context.Workers.RemoveRange(WorkersListToRemove);

                    //foreach (var item in WorkersListToRemove)
                    //{

                    //    DeleteDirectory(item.Id.ToString());

                    //    Context.Workers.Remove(item);

                    //}
                    //try
                    //{


                    //    Context.SaveChanges();

                    //}
                    //catch (Exception ex)
                    //{


                    //}


                    //var WorkersList = Context.Workers.Include(x => x.UserManager).Where(x => x.IsNew == isnew && x.UserId == CurrentUserId) .OrderByDescending(x => x.DateRigster).ToList();

                  

                    var WorkersList = (from w1 in Context.Workers.Where(x => x.FarmId == CurrentFarmId && x.StatusId==1 && (!string.IsNullOrEmpty(x.FirstName.Trim()) || !string.IsNullOrEmpty(x.LastName.Trim()) || !string.IsNullOrEmpty(x.Taz.Trim()))).DefaultIfEmpty()
                                       from w1011 in Context.Workers101.Where(x => x.IsNew == isnew && x.WorkersId == w1.Id && x.UserId==CurrentUserId).DefaultIfEmpty()
                                       from u in Context.Users.Where(x => x.Id == w1011.UserId).DefaultIfEmpty()
                                       select new WorkersWith101
                                       {
                                           w=w1,
                                           w101 = w1011,
                                           ManagerName = (u != null) ? (u.FirstName + " " + u.LastName) : null

                                       }).OrderByDescending(x => x.w101.DateRigster).ToList();



                    //.Select(x => new WorkersThin{Id =x.Id, x.FirstName,x.LastName,x.ManagerName,x.Status,x.PhoneSelular,x.DateRigster,x.IsSendSMS}).ToList();

                    return WorkersList.Where(x => x.w != null).ToList();
                }
                else
                {



                    //var WorkersListToRemove = Context.Workers.Where(x => x.IsNew == isnew && x.UserManager.Farm_Id == CurrentFarmId && (string.IsNullOrEmpty(x.FirstName) && string.IsNullOrEmpty(x.LastName) && string.IsNullOrEmpty(x.Taz))).ToList();



                    //foreach (var item in WorkersListToRemove)
                    //{

                    //    DeleteDirectory(item.Id.ToString());

                    //    Context.Workers.Remove(item);

                    //}
                    //try
                    //{


                    //    Context.SaveChanges();

                    //}
                    //catch (Exception ex)
                    //{


                    //}



                    var WorkersList = (from w1 in Context.Workers.Where(x => x.FarmId == CurrentFarmId && x.StatusId == 1 && (!string.IsNullOrEmpty(x.FirstName.Trim()) || !string.IsNullOrEmpty(x.LastName.Trim()) || !string.IsNullOrEmpty(x.Taz.Trim()))).DefaultIfEmpty()
                                       from w1011 in Context.Workers101.Where(x => x.IsNew == isnew && x.WorkersId == w1.Id).DefaultIfEmpty()
                                       from u in Context.Users.Where(x => x.Id == w1011.UserId).DefaultIfEmpty()

                                       select new WorkersWith101
                                       {
                                           w = w1,
                                           w101 = w1011,
                                           ManagerName = (u != null) ? (u.FirstName + " " + u.LastName) : null

                                       }).OrderByDescending(x => x.w101.DateRigster).ToList();


                    return WorkersList.Where(x=>x.w!=null).ToList();

                    //return Context.Workers.Include(x => x.UserManager).Where(x => x.IsNew == isnew && x.UserManager.Farm_Id == CurrentFarmId && (!string.IsNullOrEmpty(x.FirstName.Trim()) || !string.IsNullOrEmpty(x.LastName.Trim()) || !string.IsNullOrEmpty(x.Taz.Trim()))).OrderByDescending(x => x.DateRigster).ToList();

                }

            }
        }

        public static void DeleteDirectory(string Id)
        {
            var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + Id);
            if (Directory.Exists(BaseLinkSite))
            {
                DirectoryInfo di = new DirectoryInfo(BaseLinkSite);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                //foreach (DirectoryInfo dir in di.GetDirectories())
                //{
                di.Delete(true);
                // }


            }
        }

        public static WorkersWith101 GetWorker(int id)
        {


            using (var Context = new Context())
            {

                if (id >= 0)
                {
                    if (id == 0)
                    {

                        User user = GetCurrentUser();

                        Workers newWork = new Workers();
                        //newWork.UserId = user.Id;
                        //newWork.IsNew = true;
                        //newWork.DateRigster = DateTime.Now;
                        newWork.FarmId = user.Farm_Id;
                        newWork.StatusId = 1;
                        Context.Workers.Add(newWork);
                        Context.SaveChanges();
                        id = newWork.Id;

                        Workers101 W101 = new Workers101();
                        W101.UserId = user.Id;
                        W101.IsNew = true;
                        W101.DateRigster = DateTime.Now;
                        W101.WorkersId = id;
                        W101.UniqNumber = id.ToString();
                        //newWork.FarmId = user.Farm_Id;
                        Context.Workers101.Add(W101);
                        Context.SaveChanges();




                        AddToLogDB("", "", " הקמת עובדת חדשה " + id, null, "", id);
                        // return newWork;


                    }



                    //var Worker = Context.Workers.Include(x => x.UserManager).Where(x => x.Id == id).FirstOrDefault();


                    var Worker = (from w1 in Context.Workers.Where(x => x.Id == id).DefaultIfEmpty()
                                       from w1011 in Context.Workers101.Where(x =>  x.WorkersId == w1.Id).DefaultIfEmpty()
                                  from u in Context.Users.Where(x => x.Id == w1011.UserId).DefaultIfEmpty()
                                  select new WorkersWith101
                                       {
                                           w = w1,
                                           w101 = w1011,
                                      ManagerName = u.FirstName + " " + u.LastName

                                  }).OrderByDescending(x => x.w101.DateRigster).FirstOrDefault();



               


                    //.Select(x => new WorkersThin{Id =x.Id, x.FirstName,x.LastName,x.ManagerName,x.Status,x.PhoneSelular,x.DateRigster,x.IsSendSMS}).ToList();

                    //return WorkersList;



                    return Worker;

                }
                else
                {
                    var CurrentUserId = GetCurrentUser().Id;

                    var Worker = (from w1 in Context.Workers.Where(x => x.Id == CurrentUserId).DefaultIfEmpty()
                                  from w1011 in Context.Workers101.Where(x => x.WorkersId == w1.Id).DefaultIfEmpty()
                                  from u in Context.Users.Where(x => x.Id == w1011.UserId).DefaultIfEmpty()
                                  select new WorkersWith101
                                  {
                                      w = w1,
                                      w101 = w1011,
                                      ManagerName = u.FirstName + " " + u.LastName

                                  }).OrderByDescending(x => x.w101.DateRigster).FirstOrDefault();


                    return Worker;

                    //return Context.Workers.Include(x => x.UserManager).SingleOrDefault(u => u.Id == CurrentUserId);

                }




            }
        }

        public static List<WorkersWith101> DeleteWorker(int Id, bool isnew)
        {
            using (var Context = new Context())
            {
                var Worker = Context.Workers.SingleOrDefault(u => u.Id == Id);

                AddToLogDB("", "", " מחיקת עובדת " + Worker.Id, null, "", Worker.Id);


                Context.Workers.Remove(Worker);

                Context.SaveChanges();


                DeleteDirectory(Id.ToString());


                return GetWorkers(isnew);
            }
        }

        public static List<WorkerChilds> GetWorkerChilds(int Id)
        {
            using (var Context = new Context())
            {
                var WorkerChilds = Context.WorkerChilds.Where(x => x.WorkerId == Id).ToList();

                return WorkerChilds;


            }
        }

        public static WorkersWith101 UpdateWorkerAndFiles(JArray dataObj, int type)
        {

            WorkersWith101 workersWith101 = UpdateWorker(dataObj[0].ToObject<WorkersWith101>());

            List<Files> f = dataObj[1].ToObject<List<Files>>();
            if (f != null) UpdateFilesObject(f, workersWith101.w);

            List<WorkerChilds> wc = dataObj[2].ToObject<List<WorkerChilds>>();
            if (wc != null) UpdateWorkerChildsObject(wc, workersWith101.w);


            using (var Context = new Context())
            {

                try
                {


                    if (type == 2 || type == 3)
                    {
                        PdfAPI pa = new PdfAPI();

                        if (workersWith101.w101.IsNew)
                        {
                            if (type == 2) AddToLogDB("", "", " יצירת פדפ לעובד/ת חדשה  " + workersWith101.w.Id, null, "", workersWith101.w.Id);
                            //pa.CreatePDF(workersWith101);
                            pa.CreateNewCompanyPDF(workersWith101.w.FarmId, -1, workersWith101);
                        }

                        //else
                        //{
                        //    if (type == 2) AddToLogDB("", "", " יצירת פדפ לעובדת קיים  " + workersWith101.w.Id, null, "", workersWith101.w.Id);
                        //    pa.CreatePDFOnly101(workersWith101);
                        //}
                    }

                    //אם זה שמירה ושליחה למשרד
                    if (type == 2)
                    {

                        try
                        {



                            var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Workers/" + workersWith101.w.Id);

                            if (!File.Exists(BaseLinkSite + "/Signature.png"))
                            {
                                workersWith101.w101.Status = "לא ניתן לשלוח ללא חתימת עובדת";
                                return workersWith101;
                            }
                            else
                            {
                                bool IsEmpty = IsBlank(BaseLinkSite + "/Signature.png");
                                if (IsEmpty)
                                {
                                    workersWith101.w101.Status = "לא ניתן לשלוח ללא חתימת עובדת";
                                    return workersWith101;
                                }
                            }


                            // אם עובדת חדש תשלח למשרד
                            if (workersWith101.w101.IsNew)
                            {

                                var Farm = Context.Farms.Where(x => x.Id == workersWith101.w.FarmId).FirstOrDefault();

                                var CurrentUser = GetCurrentUser();

                                string CompanyEmails = null;

                                if(Farm.OfficeIsMail && !string.IsNullOrEmpty(Farm.OfficeMail))
                                {
                                    CompanyEmails = Farm.OfficeMail + ";";
                                }
                                if (Farm.ContactIsMail && !string.IsNullOrEmpty(Farm.ContactMail))
                                {
                                    CompanyEmails += Farm.ContactMail + ";";
                                }


                                if (CompanyEmails == null)
                                {
                                    workersWith101.w101.Status = "לא מוגדר מייל חברה";
                                    return workersWith101;

                                }

                                string MailTo = CompanyEmails;//ConfigurationSettings.AppSettings["MailTo"].ToString();

                            // צחי עדכן שזה יישלח לעובד עצמו במידה ויש לו מייל
                            if (!string.IsNullOrEmpty(workersWith101.w.Email))
                                {
                                    MailTo = MailTo + "," + workersWith101.w.Email;
                                }



                                string SmtpHost = ConfigurationSettings.AppSettings["SmtpHost"].ToString();
                                string MailUser = ConfigurationSettings.AppSettings["MailUser"].ToString();
                                string MailPassword = ConfigurationSettings.AppSettings["MailPassword"].ToString();



                                SmtpClient client = new SmtpClient(SmtpHost, 25);
                                client.Credentials = new System.Net.NetworkCredential(MailUser, MailPassword); //
                                client.EnableSsl = false;

                                string Body = "<html dir='rtl'><div style='text-align:right'><b>שלום רב,</b>" + "<br/>" + "מצ''ב קובץ עובד/ת חדשה.</div><br/>";// </html>";

                                Body += " מנהל/ת אזור -  " + CurrentUser.FirstName + " " + CurrentUser.LastName + "</html>";

                                string Title = "עובד/ת חדשה - " + workersWith101.w.FirstName + " " + workersWith101.w.LastName + " - " + workersWith101.w.Taz;

                                MailMessage actMSG = new MailMessage(
                                                        MailUser,
                                                        MailTo,
                                                        Title,
                                                         Body);


                                actMSG.IsBodyHtml = true;
                                //  var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + w.Id);
                                Attachment attachment = new Attachment(BaseLinkSite + "/-1/AllPdfTemp.pdf");

                                actMSG.Attachments.Add(attachment);
                                client.Send(actMSG);

                                workersWith101.w101.Status = "נשלח למשרד";

                                AddToLogDB("", "", " שליחה למשרד של עובד/ת חדשה  " + workersWith101.w.Id, null, "", workersWith101.w.Id);
                            }
                            //101 שנתי 
                            else
                            {

                                // string ManagerFile =
                                var BaseLinkSite101 = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Workers/Years_" + workersWith101.w101.ShnatMas.ToString());
                                if (!Directory.Exists(BaseLinkSite101))
                                {
                                    Directory.CreateDirectory(BaseLinkSite101);

                                }

                                string WorkerPath = BaseLinkSite101 + "/" + workersWith101.w101.UserId;

                                if (!Directory.Exists(WorkerPath))
                                {
                                    Directory.CreateDirectory(WorkerPath);



                                }

                                File.Copy(BaseLinkSite + "/-1/AllPdfTemp.pdf", WorkerPath + "/" + workersWith101.w.Id + ".pdf", true);
                                // string filePath = WorkerPath + "\\Signature.png";

                                workersWith101.w101.Status = "נשלח למשרד";

                                AddToLogDB("", "", " שליחה למשרד של עובד/ת קיימת  " + workersWith101.w.Id, null, "", workersWith101.w.Id);
                            }


                        }
                        catch (Exception ex)
                        {
                            workersWith101.w101.Status = "תקלה שליחת נתונים";
                            AddToLogDB("", "", " תקלה שליחה למשרד של עובדת  " + workersWith101.w.Id, null, ex.Message, workersWith101.w.Id);
                            // w.Status = ex.InnerException.ToString();
                        }
                        finally
                        {


                            Context.Entry(workersWith101.w).State = System.Data.Entity.EntityState.Modified;
                            Context.Entry(workersWith101.w101).State = System.Data.Entity.EntityState.Modified;
                            Context.SaveChanges();

                            


                        }

                    }

                }
                catch (Exception ex)
                {
                    AddToLogDB("", "", " תקלה שליחה למשרד של עובדת  " + workersWith101.w.Id, null, ex.Message, workersWith101.w.Id);

                }
            }
            return workersWith101;
        }

        public static bool IsBlank(string imageFileName)
        {
            double stdDev = GetStdDev(imageFileName);
            return stdDev < 10;
        }

        /// <summary>
        /// Get the standard deviation of pixel values.
        /// </summary>
        /// <param name="imageFileName">Name of the image file.</param>
        /// <returns>Standard deviation.</returns>
        public static double GetStdDev(string imageFileName)
        {
            double total = 0, totalVariance = 0;
            int count = 0;
            double stdDev = 0;

            // First get all the bytes
            using (Bitmap b = new Bitmap(imageFileName))
            {
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
                int stride = bmData.Stride;
                IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * 3;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            count++;
                            byte blue = p[0];
                            byte green = p[1];
                            byte red = p[2];

                            int pixelValue = red + green + blue;
                            total += pixelValue;
                            double avg = total / count;
                            totalVariance += Math.Pow(pixelValue - avg, 2);
                            stdDev = Math.Sqrt(totalVariance / count);

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                b.UnlockBits(bmData);
            }

            return stdDev;
        }




        private static void CreatePDF(Workers w)
        {
            throw new NotImplementedException();
        }

        private static void UpdateWorkerChildsObject(List<WorkerChilds> objList, Workers w)
        {
            using (var Context = new Context())
            {

                foreach (WorkerChilds item in objList)
                {

                    item.WorkerId = w.Id;

                    if (item.Id == 0)
                    {
                        Context.WorkerChilds.Add(item);
                        AddToLogDB("", "", " הקמת ילד לעובדת " + item.Id, null, "", item.Id);

                    }
                    else
                    {

                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;


                    }

                }

                try
                {

                    var result = Context.WorkerChilds.Where(p => p.WorkerId == w.Id).ToList();
                    IEnumerable<WorkerChilds> differenceQuery = result.Except(objList);

                    foreach (WorkerChilds item in differenceQuery)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                        AddToLogDB("", "", " מחיקת ילד לעובדת " + item.Id, null, "", item.Id);
                    }



                }
                catch (Exception ex)
                {


                }

                Context.SaveChanges();


            }
        }



        private static void UpdateFilesObject(List<Files> objList, Workers w)
        {
            using (var Context = new Context())
            {

                foreach (Files item in objList)
                {

                    item.WorkerId = w.Id;

                    if (item.Id == 0)
                    {
                        Context.Files.Add(item);

                        AddToLogDB("", "", " הוספת קובץ לעובדת " + item.Id, null, "", item.Id);

                    }
                    else
                    {

                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;


                    }

                }

                try
                {

                    var result = Context.Files.Where(p => p.WorkerId == w.Id).ToList();
                    IEnumerable<Files> differenceQuery = result.Except(objList);

                    foreach (Files item in differenceQuery)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Deleted;

                        AddToLogDB("", "", " מחיקת קובץ לעובדת " + item.Id, null, "", item.Id);
                    }



                }
                catch (Exception ex)
                {


                }

                Context.SaveChanges();


            }
        }


        public static WorkersWith101 UpdateWorker(WorkersWith101 WorkersWith101)
        {
            //  System.Threading.Thread.Sleep(5000);

            using (var Context = new Context())
            {


                Workers w = WorkersWith101.w;
                Workers101 w101 = WorkersWith101.w101;

                w101.Status = "נתונים נשמרו";

                w101.UserManager = Context.Users.Where(x => x.Id == w101.UserId).FirstOrDefault();

                Context.Entry(w101).State = System.Data.Entity.EntityState.Modified;
                Context.Entry(w).State = System.Data.Entity.EntityState.Modified;
                Context.SaveChanges();
                AddToLogDB("", "", " שמירת נתונים לעובדת " + w.Id, null, "", w.Id);

                if (!string.IsNullOrEmpty(w101.ImgData))
                {

                    string root = HttpContext.Current.Server.MapPath("~/Uploads/Workers/");

                    string WorkerPath = root + w.Id.ToString();

                    if (!Directory.Exists(WorkerPath))
                    {
                        Directory.CreateDirectory(WorkerPath);

                    }
                    string filePath = WorkerPath + "\\Signature.png";
                 

                    File.WriteAllBytes(filePath, GetValidString(w101.ImgData));

                    AddToLogDB("", "", " הוספת חתימה לעובדת " + w.Id, null, "", w.Id);
                }

                return WorkersWith101;
            }
        }

        public static byte[] GetValidString(string s)
        {
            s = s.Replace("data:image/png;base64,", "");
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return Convert.FromBase64String(s);
        }

        public static string DecryptString(string Val)
        {
           return AesOperation.DecryptString(Val);
        }


        public static List<WorkersWith101> SendSMS(List<WorkersWith101> WorkersItems, int IsNew)
        {

            string SiteRegisterLink = ConfigurationSettings.AppSettings["SiteRegisterLink"].ToString();

            using (var Context = new Context())
            {
                foreach (var item in WorkersItems)
                {

                    var Phone = item.w.PhoneSelular;
                    var Id = item.w.Id;
                    var FullName = item.w.FullName;
                    string EncryptId = AesOperation.EncryptString(Id.ToString());

                    EncryptId = EncryptId.Replace("+", "@@").Replace("/", "ofekslash");

                    //string DecryptId = AesOperation.DecryptString(EncryptId);



                    if (!string.IsNullOrEmpty(Phone) && Phone.Length > 7)
                    {
                        var Message = string.Format("שלום רב {0}\r\nלהשלמת הטופס ולחתימה על 101 לחץ כאן:\r\n{1}\r\n", FullName, SiteRegisterLink + EncryptId + "/");

                        var res = SendSMSEndPoint(Phone, Message);

                        var resObj = ResAsJson(res);

                        if (resObj["success"] == "true")
                        {
                            item.w101.IsSendSMS = true;
                            
                            Context.Entry(item).State = System.Data.Entity.EntityState.Modified;

                        }
                    }

                }

                Context.SaveChanges();
            }

            return GetWorkers(true);

        }


        public static dynamic ResAsJson(string jsonString)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return jsonObj;


        }


        private static string SendSMSEndPoint(string Phone, string message)
        {

            if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(message)) return "{'success':'false','message':'no Phone or no message sent.'}";


            RestSharp.RestClient val = new RestSharp.RestClient("https://api.multisend.co.il/v2/sendsms")
            {

                Timeout = -1

            };

            RestSharp.RestRequest val2 = new RestSharp.RestRequest((RestSharp.Method)1);

            val2.AddHeader("Content-Type", "multipart/form-data; boundary=--------------------------486507896899304374172095");

            //val2.AlwaysMultipartFormData = true;

            val2.AddParameter("user", (object)"shmiatech");

            val2.AddParameter("password", (object)"shmiatech@01");

            val2.AddParameter("from", (object)"101Form");

            val2.AddParameter("recipient", (object)Phone);

            val2.AddParameter("message", (object)message);

            val2.AddParameter("message_type", (object)"SMS");

            RestSharp.IRestResponse val3 = val.Execute((RestSharp.IRestRequest)(object)val2);

            return val3.Content;



            //InsertLog("INFO", "SendSmsTtsNew - " + message_type, Phone, "Response: " + val3.Content);

        }






        //public static string SendSMSEndPoint(string Phone, string Message)
        //{
        //    try
        //    {


        //        if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Message)) return "{'success':'false','message':'no Phone or no message sent.'}";

        //        var options = new RestClientOptions("https://api.multisend.co.il")
        //        {
        //            MaxTimeout = -1,
        //        };
        //        var client = new RestClient(options);

        //        var request = new RestRequest("/v2/sendsms/", Method.Get);

        //        request.AddHeader("Content-Type", "application/json");

        //        request.AddParameter("user", "bpreven");

        //        request.AddParameter("password", "Bpreven@3817");

        //        request.AddParameter("from", "Bpreven");

        //        request.AddParameter("recipient", Phone);

        //        request.AddParameter("message", Message);

        //        request.AddParameter("message_type", "SMS");



        //        RestResponse response = client.Execute(request);
        //        //only throws the exception. Let target choose what to do
        //        if (response.ErrorException != null)
        //        {

        //            //if (_context != null)
        //            //{
        //            //    Logs l = new Logs();

        //            //    l.Text = response.ErrorException.ToString();

        //            //    _context.Logs.Add(l);
        //            //    _context.SaveChanges();


        //            //}
        //            return "{'success':'false','message':" + response.ErrorException.ToString() + "}";

        //           //return response.ErrorException.ToString();
        //        }




        //        return response.Content;


        //    }
        //    catch (Exception ex)
        //    {
        //        //if (_context != null)
        //        //{
        //        //    Logs l = new Logs();

        //        //    l.Text = ex.Message.ToString();

        //        //    _context.Logs.Add(l);
        //        //    _context.SaveChanges();


        //        //}

        //        return "{'success':'false','message':" +ex.Message +"}";

        //    }




        //}



        //********************* End Workers ***************************
        //********************* Master Table ***************************

        public static List<Cities> GetCitiesList()
        {


            using (var Context = new Context())
            {

                return Context.Cities.ToList();

            }
        }

        public static List<Banks> GetBanksList()
        {


            using (var Context = new Context())
            {

                return Context.Banks.ToList();

            }
        }

        public static List<BanksBrunchs> GetBanksBrunchsList()
        {


            using (var Context = new Context())
            {

                return Context.BanksBrunchs.ToList();

            }
        }


        //********************* End Master Table ***************************


        public static User UpdateUser(User User)
        {
            using (var Context = new Context())
            {
                var CurrentUserFarmId = GetCurrentUser().Farm_Id;
                User.Farm_Id = CurrentUserFarmId != 0 ? CurrentUserFarmId : User.Farm_Id;
                if (User.Role == "sysAdmin")
                {
                    User.Farm_Id = 0;
                }
                var UserIdByEmail = GetUserIdByEmail(User.Email, CurrentUserFarmId);
                if (User.Id == 0 && UserIdByEmail == 0)
                {
                    Context.Users.Add(User);
                    Context.SaveChanges();
                }

                // צחי הוסיף בכדי למנוע עדכון של ת"ז קיים לחווה מסויימת
                if (User.Id != UserIdByEmail && UserIdByEmail != 0)
                {
                    User.FirstName = "Error";
                    return User;
                }



                //// צחי שינה
                //var Meta = JObject.Parse(User.Meta);
                //if (Meta["AvailableHours"] != null)
                //{
                //    foreach (var Item in Meta["AvailableHours"])
                //    {
                //        Item["resourceId"] = User.Id;
                //    }
                //}
                //User.Meta = Meta.ToString(Formatting.None);

                Context.Entry(User).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    Context.SaveChanges();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // צחי הוסיף כדי לבדוק אם קיים כבר תלמיד כזה להחזיר שגיאה למשתמש
                    User.FirstName = "Error";
                }

                return User;
            }
        }

        public static void DeleteUser(int Id)
        {
            using (var Context = new Context())
            {
                var User = Context.Users.SingleOrDefault(u => u.Id == Id);
                if (User != null)
                {
                    User.Deleted = true;
                }
                //  Context.Notifications.RemoveRange(Context.Notifications.Where(n => n.EntityId == Id && n.EntityType == "student"));
                Context.SaveChanges();
            }
        }

        public static void DestroyUser(string email)
        {
            using (var Context = new Context())
            {
                var User = Context.Users.SingleOrDefault(u => u.Email == email);
                if (User != null)
                {

                    Context.Users.Remove(User);
                }
                //  Context.Notifications.RemoveRange(Context.Notifications.Where(n => n.EntityId == User.Id && n.EntityType == "student"));
                Context.SaveChanges();
            }
        }

        public static int GetUserIdByEmail(string Email, int CurrentUserFarmId = 0)
        {
            using (var Context = new Context())
            {

                var User = Context.Users.SingleOrDefault(u => u.Email.ToLower() == Email.ToLower() && (CurrentUserFarmId == 0 || u.Farm_Id == CurrentUserFarmId));
                if (User != null)
                    return User.Id;
                return 0;
            }
        }

        public static User GetCurrentUser()
        {

            //StackTrace stackTrace = new StackTrace();
            //// Get calling method name
            //Console.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);

            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;

            if (identity.Claims.Count()== 0)
            return GetUser(GetUserIdByEmail("default@gmail.com"));
            
            var Email = identity.Claims.SingleOrDefault(c => c.Type == "sub").Value;
            return GetUser(GetUserIdByEmail(Email));
        }

        public static void RegisterDevice(string token)
        {
            using (var Context = new Context())
            {
                try
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    //if (Context.UserDevices.SingleOrDefault(ud => ud.DeviceToken == token && ud.User_Id == CurrentUserId) == null)
                    //{
                    //    Context.UserDevices.Add(new UserDevices() { User_Id = CurrentUserId, DeviceToken = token });
                    //    Context.SaveChanges();
                    //}
                }
                catch (Exception ex) { }
            }
        }

        public static void UnregisterDevice(string token)
        {
            using (var Context = new Context())
            {
                try
                {
                    var CurrentUserId = GetCurrentUser().Id;
                    //var UserDevice = Context.UserDevices.Where(ud => ud.DeviceToken == token);
                    //Context.UserDevices.RemoveRange(UserDevice);
                    Context.SaveChanges();
                }
                catch (Exception ex) { }
            }
        }

        public static List<string> GetDevices(string UserId)
        {
            using (var Context = new Context())
            {
                try
                {
                    return null;
                    //  return Context.UserDevices.Where(ud => ud.User_Id == int.Parse(UserId)).Select(ud => ud.DeviceToken).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static List<Report> GetReportData(int type)
        {
            using (var Context = new Context())
            {
                try
                {
                    SqlParameter TypePara = new SqlParameter("Type", 1);
                    SqlParameter YearPara = new SqlParameter("Year", DateTime.Now.Year);

                    var query = Context.Database.SqlQuery<Report>
                    ("GetReportData @Type,@Year", TypePara, YearPara);



                    var res = query.ToList();


                    return res;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static string DownloadAllManagerFiles(int Id, int Shnatmas)
        {
            using (var Context = new Context())
            {

                var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Years_" + Shnatmas + "/" + Id + "/");
                // var ZipLink = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Years_" + Shnatmas + "/" + Id + "/");
                var ZipPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Years_" + Shnatmas + "/" + Id + ".zip");
                if (Directory.Exists(BaseLinkSite))
                {

                    // ZipArchive ddd = new ZipArchive();
                    if (File.Exists(ZipPath))
                    {
                        File.Delete(ZipPath);

                    }
                    ZipFile.CreateFromDirectory(BaseLinkSite, ZipPath);

                    return "1";

                }



                return "0";
            }
        }

        public static void AddEnterLog(JObject dataobj)
        {

            string DeviceEnter = dataobj["DeviceEnter"].ToString();
            string UserAgent = dataobj["UserAgent"].ToString();
            string Action = "נכנס לאתר";
            AddToLogDB(DeviceEnter, UserAgent, Action, null, "", 0);


        }


        public static void ImportWorkers(List<Workers> WorkersItems, int counter)
        {




            //string ShnatMas = DateTime.Now.Year.ToString();

            //DateTime CurrentDate = DateTime.Now;

            //using (var Context = new Context())
            //{
            //    var existingUniqNumber = Context.Workers.Where(x => x.UniqNumber != null && x.ShnatMas == ShnatMas).Select(x => x.UniqNumber).ToList();

            //    var nonExistWorkers = WorkersItems.Where(x => !existingUniqNumber.Contains(x.UniqNumber)).ToList();



            //    nonExistWorkers.Select(c => { c.DateRigster = CurrentDate; return c; }).ToList();

            //    Context.Workers.AddRange(nonExistWorkers);

            //    Context.SaveChanges();

                


            //}
        }

        public static List<Logs> GetLogsData(int userid, string start, string end)
        {
            DateTime dtStart = Convert.ToDateTime(start);
            DateTime dtSEnd = Convert.ToDateTime(end);
            using (var Context = new Context())
            {
                var LogsList = Context.Logs.Where(u => u.UserId == userid && u.DateTime <= dtSEnd && u.DateTime >= dtStart).OrderBy(x => x.DateTime).ToList();
                return LogsList;
            }
        }
        private static void AddToLogDB(string deviceEnter, string userAgent, string action, User u, string Expetion, int WorkerId)
        {
            if (u == null)
            {
                u = GetCurrentUser();
                //try
                //{
                //    u = GetCurrentUser();
                //}
                //catch (Exception ex)
                //{
                //    u = new User();
                //    u.FirstName = "עובד";
                //    u.LastName = "";
                //}
            }

            using (var Context = new Context())
            {
                //  if (WorkerId != 0)
                var WorkerObj = Context.Workers.Where(x => x.Id == WorkerId).FirstOrDefault();
                Logs l = new Logs();
                l.Device = deviceEnter;
                l.UserAgent = userAgent;
                l.Action = action;
                l.DateTime = DateTime.Now;
                l.Expetion = Expetion;
                l.UserId = u.Id;
                l.UserName = u.FirstName + ' ' + u.LastName;
                if (WorkerObj != null) l.WorkerName = WorkerObj.FirstName + " " + WorkerObj.LastName;
                Context.Logs.Add(l);
                Context.SaveChanges();


            }
        }

        // UsersService.AddEnterLog(dataobj)




        #region Helpers

        public static List<User> RemovePassword(List<User> Users)
        {
            foreach (var User in Users)
                User.Password = null;

            return Users;
        }

        public static User RemovePassword(User User)
        {
            User.Password = null;
            return User;
        }

      

        #endregion
    }


    public class AesOperation
    {
        public static string key = "b14ca5898a4e4133bbce2ea2315a1916";
        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
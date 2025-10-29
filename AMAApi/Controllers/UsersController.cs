using FarmsApi.DataModels;
using Google.Rpc;
using Grpc.Core;
using iTextSharp.tool.xml.html;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace FarmsApi.Services
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {

        [Route("DeleteYear")]
        [HttpGet]
        public string DeleteYear()
        {
            using (var Context = new Context())
            {
                ////סתם בירבורים
                //var Wor = Context.Workers.Where(x => x.ShnatMas == "2021").ToList();
                //foreach (var item in Wor)
                //{
                //    DeleteWorkerLoop(item.Id,true);
                //}


            }

            //UploadFromAccess uac = new UploadFromAccess();
            //uac.UpdateUsersLessons();
            return "sdsdsdsd";


        }


        public static void DeleteWorkerLoop(int Id, bool isnew)
        {
            using (var Context = new Context())
            {
                var Worker = Context.Workers.SingleOrDefault(u => u.Id == Id);


                Context.Workers.Remove(Worker);

                Context.SaveChanges();


                UsersService.DeleteDirectory(Id.ToString());



            }
        }











        [Authorize]
        [Route("getUsers/{role?}/{includeDeleted?}")]
        [HttpGet]
        public IHttpActionResult GetUsers(string role = null, bool includeDeleted = false)
        {
            return Ok(UsersService.GetUsers(role, includeDeleted));
        }

        [Authorize]
        [Route("getUser/{id?}")]
        [HttpGet]
        public IHttpActionResult GetUser(int? id = null)
        {
            return Ok(UsersService.GetUser(id));
        }

        [Authorize]
        [Route("getsetUserEnter/{isForCartis}/{id?}")]
        [HttpGet]
        public IHttpActionResult GetSetUserEnter(int? id = null, bool isForCartis = false)
        {
            return Ok(UsersService.GetSetUserEnter(id, isForCartis));
        }



        [Authorize]
        [Route("newUser")]
        [HttpGet]
        public IHttpActionResult NewUser()
        {
            return Ok(new User());
        }

        [Authorize]
        [Route("getUserIdByEmail/{email}")]
        [HttpGet]
        public IHttpActionResult GetUserIdByEmail(string email)
        {
            return Ok(UsersService.GetUserIdByEmail(email));
        }

        [Authorize(Roles = "farmAdmin,farmAdminHorse,sysAdmin,vetrinar,shoeing")]
        [Route("deleteUser/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteUser(int id)
        {
            UsersService.DeleteUser(id);
            return Ok();
        }

        [Authorize(Roles = "farmAdmin,farmAdminHorse,sysAdmin,vetrinar,shoeing")]
        [Route("destroyUser")]
        [HttpGet]
        public IHttpActionResult DestroyUser([FromUri] string email)
        {
            UsersService.DestroyUser(email);
            return Ok();
        }

        [Authorize]
        [Route("updateUser")]
        [HttpPost]
        public IHttpActionResult UpdateUser(DataModels.User user)
        {
            return Ok(UsersService.UpdateUser(user));
        }


        [Authorize]
        [Route("getPortfolios/{llx}/{lly}/{urx}/{ury}/{text}/{font}/{space}/{id}/{pagenumber}")]
        [HttpGet]
        public IHttpActionResult GetPortfolios(int llx, int lly, int urx, int ury, string text, int font, int space, int id, int pagenumber)
        {
            return Ok(UsersService.GetPortfolios(llx, lly, urx, ury, text, font, space, id, pagenumber));
        }

        [Authorize]
        [Route("bindData/{id}/{comment}/{pagenumber}/{value}")]
        [HttpGet]
        public IHttpActionResult BindData(int id, string comment, int pagenumber, string value)
        {
            return Ok(UsersService.BindData(id, comment, pagenumber, value));
        }


        //******************************************** Workers *****************************
        //[Authorize]
        [Route("getFiles/{workerid}")]
        [HttpGet]
        public IHttpActionResult GetFiles(string Workerid)
        {

            string res = Workerid;

            if (Regex.Matches(Workerid, @"[a-zA-Z]").Count > 0)
            {
                Workerid = Workerid.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(Workerid);
            }
            return Ok(UsersService.GetFiles(Convert.ToInt32(res)));
            //return Ok(UsersService.GetFiles(Workerid));
        }


        //[Authorize]
        [Route("getWorkerChilds/{id}")]
        [HttpGet]
        public IHttpActionResult GetWorkerChilds(string id)
        {
            string res = id;

            if (Regex.Matches(id, @"[a-zA-Z]").Count > 0)
            {
                id = id.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(id);
            }
            return Ok(UsersService.GetWorkerChilds(Convert.ToInt32(res)));
        }



        // [Authorize]
        [Route("getWorker/{id}/{campainid}/{shnatmas}/")]
        [HttpGet]
        public IHttpActionResult GetWorker(string id,int campainid=-1,string shnatmas=null)
        {
            string res = id;

            if (Regex.Matches(id, @"[a-zA-Z]").Count > 0)
            {
                id = id.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(id);
            }
            return Ok(UsersService.GetWorker(Convert.ToInt32(res), campainid, shnatmas));
        }


        [Route("getWorkerAll/{id}/{type?}")]
        [HttpGet]
        public IHttpActionResult GetWorkerAll(string id,int type=1)
        {
            string res = id;

            if (Regex.Matches(id, @"[a-zA-Z]").Count > 0)
            {
                id = id.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(id);
            }



            using (var Context = new Context())
            {

                int newId = Convert.ToInt32(res);


                //מחזיר את המסמכים של העובד
                if (type == 2)
                {

                     var CurrentDate =  DateTime.Now;


                    var Company = Context.Workers.Where(x => x.Id == newId).FirstOrDefault();


                    if (Company == null) return Ok();

                   // x.FarmId == Company.FarmId &&
                    var CampainsUsers = (
                                         from c in Context.Campains.Where(x => (x.FarmId == Company.FarmId || x.FarmId ==-1) && x.StatusId==1 && (!x.DateValidity.HasValue || (x.DateValidity.HasValue && x.DateValidity.Value >= CurrentDate)))
                                         from cs in Context.CampainsStatus.Where(x => x.WorkersId == newId && c.Id==x.CampainsId)
                                         from cst in Context.CampainsStatusType.Where(x => x.Id == cs.StatusId).DefaultIfEmpty()
                                         from farmpdffiles in Context.FarmPDFFiles.Where(x => x.CampainsId == c.Id && x.StatusId==1 && x.Is101).DefaultIfEmpty()
                                             // where  !c.DateValidity.HasValue || (c.DateValidity.HasValue && c.DateValidity <= CurrentDate)

                                         select new 
                                  {
                                      cs,
                                      c,
                                      cst,
                                      Is101 = (farmpdffiles==null)?false:true


                                         }).OrderBy(x => x.cs.StatusId).ThenByDescending(x => x.c.DateRigster).ToList();

                    return Ok(CampainsUsers);
                }




                if (newId >= 0)
                {
                    if (newId == 0)
                    {

                        User user = Helper.GetCurrentUser();

                        Workers newWork = new Workers();

                        newWork.FarmId = user.FarmId;
                        newWork.StatusId = 1;
                        Context.Workers.Add(newWork);
                        Context.SaveChanges();
                        newId = newWork.Id;




                    }

                    var Worker = (from w1 in Context.Workers.Where(x => x.Id == newId).DefaultIfEmpty()

                                  select new WorkersWith101
                                  {
                                      w = w1


                                  }).OrderByDescending(x => x.w.Id).FirstOrDefault();

                    return Ok(Worker);

                }
                

                return Ok();

            }


        }




        [Authorize]
        [Route("getWorkers/{isnew}")]
        [HttpGet]


        public IHttpActionResult GetWorkers(bool isnew, int page = 1, int pageSize = 10, string filterText = null, int statusid = -1, int factoryid = 0, int divisionsid = 0,
            int subdivisionsid = 0, int departmentsid = 0, int subdepartmentsid = 0, string status101 = null)
        {
            return Ok(UsersService.GetWorkers(isnew, page, pageSize, filterText, statusid, factoryid, divisionsid,
             subdivisionsid, departmentsid, subdepartmentsid, status101));
        }

        [Authorize]
        [Route("getWorkersAll/{isnew}")]
        [HttpGet]


        public IHttpActionResult GetWorkersAll(bool isnew, int page = 1, int pageSize = 10, string filterText = null, int statusid = -1, int factoryid = 0, int divisionsid = 0,
         int subdivisionsid = 0, int departmentsid = 0, int subdepartmentsid = 0, string status101 = null)
        {
            using (var Context = new Context())
            {


                var CurrentUser = Helper.GetCurrentUser();

                var CurrentUserId = CurrentUser.Id;
                var CurrentRolesId = CurrentUser.RolesId;
                var CurrentFarmId = CurrentUser.FarmId;


                var DepartmentsList = Helper.GetCurrentUserPermissions();

                var AllowedDepartmentIds = DepartmentsList.Select(x => x.Id).ToList();


                //יכול להיות רק worker 
                //סתם מתלבש על המבנה
                var WorkersList = new List<WorkersWith101>();


                var CurrentTotalCount = Context.Workers.Where(x =>
                                                              x.FarmId == CurrentFarmId &&

                                                              (filterText == null || ((x.FirstName + " " + x.LastName).Contains(filterText) || x.Taz.Contains(filterText) || x.Phone.Contains(filterText))) &&
                                                              (statusid == -1 || (x.StatusId == statusid)) &&
                                                              (factoryid == 0 || (x.FactoryId == factoryid)) &&
                                                              (divisionsid == 0 || (x.DivisionsId == divisionsid)) &&
                                                              (subdivisionsid == 0 || (x.SubDivisionsId == subdivisionsid)) &&
                                                              (departmentsid == 0 || (x.DepartmentsId == departmentsid)) &&
                                                              (subdepartmentsid == 0 || (x.SubDepartmentsId == subdepartmentsid)) &&


                                                              // אם מדובר במנהל מערכת
                                                              // או בסופר אדמין
                                                              // אחרת תביא לי רק את העובדים שתחת הרשאה של מנהל מפעל או מחלקה
                                                              (CurrentRolesId == 0 ||
                                                               CurrentRolesId == 2 ||
                                                               AllowedDepartmentIds.Contains(x.FactoryId ?? -1) ||
                                                               AllowedDepartmentIds.Contains(x.DepartmentsId ?? -1)

                                                               ) &&

                                                              (!string.IsNullOrEmpty(x.FirstName.Trim()) || !string.IsNullOrEmpty(x.LastName.Trim()) || !string.IsNullOrEmpty(x.Taz.Trim()))

                                ).Count(); // או לפי Date



                WorkersList = (from w1 in Context.Workers.Where(x =>
                                                                x.FarmId == CurrentFarmId &&
                                                                //x.StatusId == 1 &&
                                                                (filterText == null || ((x.FirstName + " " + x.LastName).Contains(filterText) || x.Taz.Contains(filterText) || x.Phone.Contains(filterText))) &&

                                                                (statusid == -1 || (x.StatusId == statusid)) &&
                                                                (factoryid == 0 || (x.FactoryId == factoryid)) &&
                                                                (divisionsid == 0 || (x.DivisionsId == divisionsid)) &&
                                                                (subdivisionsid == 0 || (x.SubDivisionsId == subdivisionsid)) &&
                                                                (departmentsid == 0 || (x.DepartmentsId == departmentsid)) &&
                                                                (subdepartmentsid == 0 || (x.SubDepartmentsId == subdepartmentsid)) &&

                                                               (CurrentRolesId == 0 ||
                                                               CurrentRolesId == 2 ||
                                                               AllowedDepartmentIds.Contains(x.FactoryId ?? -1) ||
                                                               AllowedDepartmentIds.Contains(x.DepartmentsId ?? -1)

                                                               ) &&


                                                                (!string.IsNullOrEmpty(x.FirstName.Trim()) || !string.IsNullOrEmpty(x.LastName.Trim()) || !string.IsNullOrEmpty(x.Taz.Trim()))
                                                            )


                               select new WorkersWith101
                               {

                                   w = w1


                               }).OrderByDescending(x => x.w.Id)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();





                //foreach (var item in WorkersList)
                //{
                //    if (item?.w == null) continue;

                //    if (item?.w101 == null)
                //    {
                //        item.w101 = new Workers101();
                //        item.w101.UserId = item.w.Id;
                //        item.w101.ShnatMas = "2025";
                //    }
                //    string filePath = Path.Combine(basePath, item.w.Id.ToString(), "-1", "AllPdfTemp.pdf");
                //    item.HasPdf = System.IO.File.Exists(filePath);
                //}

                WorkersResult workersResult = new WorkersResult();
                workersResult.TotalCount = CurrentTotalCount;
                workersResult.Items = WorkersList;

                return Ok(workersResult);








            }
        }





        [Authorize]
        [Route("deleteWorker/{id}/{isnew}")]
        [HttpGet]
        public IHttpActionResult DeleteWorker(int id, bool isnew)
        {

            return Ok(UsersService.DeleteWorker(id, isnew));
        }





        // [Authorize]
        [Route("updateWorker/{type}/{campainid?}/")]
        [HttpPost]
        public IHttpActionResult UpdateWorkerAndFiles(JArray dataobj, int type, int campainid=-1)
        {
            return Ok(UsersService.UpdateWorkerAndFiles(dataobj, type, campainid));
        }

        // [Authorize]
        [Route("updateWorkerAll/{type}")]
        [HttpPost]
        public IHttpActionResult UpdateWorkerAll(Workers worker, int type)
        {
            using (var Context = new Context())
            {
                Context.Entry(worker).State = System.Data.Entity.EntityState.Modified;
            
                Context.SaveChanges();
                // return Ok(UsersService.UpdateWorkerAndFiles(dataobj, type));

                return Ok(worker);
            }


        }


        // [Authorize]
        [Route("setUserDevice")]
        [HttpPost]
        public IHttpActionResult SetUserDevice(JObject dataobj)
        {

            //UsersService.AddEnterLog(dataobj);
            return Ok();
        }


        [Authorize]
        [Route("sendSMS")]
        [HttpPost]
        public IHttpActionResult SendSMS(List<DataModels.WorkersWith101> WorkersItems, int type, bool isnew = true, int page = 1, int pageSize = 10, string filterText = null,
            int statusid = -1, int factoryid = 0, int divisionsid = 0,
            int subdivisionsid = 0, int departmentsid = 0, int subdepartmentsid = 0, string status101 = null

            )
        {


            // קבלת אך ורק את ההודעה מהשרת בשביל ווטסאפ
            if (type == 4 || type == 44)
            {
                var firstWorker = WorkersItems.FirstOrDefault();

                if (firstWorker != null)
                {

                    var Phone = firstWorker.w.PhoneSelular;
                    var Id = firstWorker.w.Id;
                    var FullName = firstWorker.w.FullName;
                    var Email = firstWorker.w.Email;
                    var FarmId = firstWorker.w.FarmId;


                    string EncryptId = AesOperation.EncryptString(Id.ToString());

                    EncryptId = EncryptId.Replace("+", "@@").Replace("/", "ofekslash");
                    string SiteRegisterLink = ConfigurationSettings.AppSettings["SiteRegisterLink"].ToString();

                    var Message = string.Format("שלום רב {0}\r\nלהשלמת הטופס ולחתימה על 101 לחץ כאן:\r\n{1}\r\n", FullName, SiteRegisterLink + EncryptId + "/");
                   
                    
                    if (type == 44)
                    {
                        string SiteUsersCampains = ConfigurationSettings.AppSettings["SiteUsersCampains"].ToString();

                        Message = string.Format("שלום רב {0}\r\nלצפיה בכל המסמכים שלך לחצ/י כאן:\r\n{1}\r\n", FullName, SiteUsersCampains + EncryptId + "/");



                    }
                    
                    
                    
                    return Ok(Message);
                }



            }



            return Ok(UsersService.SendSMS(WorkersItems, type, true, page, pageSize, filterText, statusid, factoryid, divisionsid,
             subdivisionsid, departmentsid, subdepartmentsid, status101));

        }


        //[Route("sendLinktoWorkers/{type}/{campainid}/")]
        //[HttpPost]
        //public IHttpActionResult sendLinktoWorkers(int type, int campainid, List<Workers> workers)
        //{


        //    using (var Context = new Context())
        //    {

        //        var CurrentDate = DateTime.Now;

        //        foreach (Workers worker in workers)
        //        {

        //            var Phone = worker.PhoneSelular;
        //            var Id = worker.Id;
        //            var FullName = worker.FullName;
        //            var Email = worker.Email;
        //            var FarmId = worker.FarmId;


        //            string EncryptId = AesOperation.EncryptString(Id.ToString());

        //            EncryptId = EncryptId.Replace("+", "@@").Replace("/", "ofekslash");

        //            //string DecryptId = AesOperation.DecryptString(EncryptId);



        //            if (!string.IsNullOrEmpty(Phone) && Phone.Length > 7)
        //            {

        //                string SiteRegisterCampain = Helper.GetConfigureValue("SiteRegisterCampain");

        //                var Message = string.Format("שלום {0}\r\nלהשלמת הטופס ולחתימה לחצ/י כאן:\r\n{1}\r\n", FullName, SiteRegisterCampain + EncryptId + "/" + campainid.ToString() + "/");

        //                Campains c = Context.Campains.Where(x => x.Id == campainid).FirstOrDefault();

        //                bool IsSendWorkers = false;

        //                // SMS
        //                if (type == 1)
        //                {


        //                    string Title = "";
        //                    if (!string.IsNullOrEmpty(c.NameEn))
        //                        Title = c.NameEn;

        //                    var res = Helper.SendSMSEndPoint(Phone, Message, Title);

        //                    var resObj = Helper.ResAsJson(res);

        //                    if (resObj["success"] == "true")
        //                    {
        //                        IsSendWorkers = true;


        //                    }

        //                }
        //                //מייל
        //                if (type == 2 && !string.IsNullOrEmpty(Email))
        //                {

        //                    bool Res = Helper.SendMail(c.Name, Message.Replace("\r\n", "<br>"), Email, "", FarmId.ToString());

        //                    if (Res)
        //                    {
        //                        IsSendWorkers = true;

        //                    }

        //                }
        //                if (type == 3)
        //                {
        //                    IsSendWorkers = true;

        //                }
        //                // קבלת אך ורק את ההודעה מהשרת
        //                if (type == 4)
        //                {
        //                    return Ok(Message);

        //                }

        //                if (IsSendWorkers)
        //                {

        //                    CampainsStatus cs = Context.CampainsStatus.Where(x => x.CampainsId == campainid && x.WorkersId == Id).FirstOrDefault();

        //                    if (cs != null)
        //                    {

        //                        if (cs.DateSend == null)
        //                        {

        //                            c.CountSend++;

        //                        }


        //                        cs.StatusId = 5;
        //                        cs.CampainsId = campainid;
        //                        cs.MediaId = type;
        //                        cs.WorkersId = Id;
        //                        cs.DateSend = CurrentDate;
        //                        Context.Entry(cs).State = System.Data.Entity.EntityState.Modified;
        //                    }
        //                    else
        //                    {

        //                        cs = new CampainsStatus();

        //                        cs.StatusId = 5;
        //                        cs.CampainsId = campainid;
        //                        cs.MediaId = type;
        //                        cs.WorkersId = Id;
        //                        cs.DateSend = CurrentDate;

        //                        Context.CampainsStatus.Add(cs);

        //                        c.CountSend++;


        //                    }

        //                    Context.Entry(c).State = System.Data.Entity.EntityState.Modified;


        //                    Context.SaveChanges();

        //                }



        //            }


        //        }



        //    }
        //    return GetSetCampainsData(4, campainid.ToString(), null);



        //}





        //******************************************** End Workers *****************************
        //******************************************** Master Table *****************************
        //[Authorize]
        [Route("getMasterTable/{type}")]
        [HttpGet]
        public IHttpActionResult GetMasterTable(int type)
        {

            switch (type)
            {
                case 1:
                    return Ok(UsersService.GetCitiesList());

                case 2:
                    return Ok(UsersService.GetBanksList());
                case 3:
                    return Ok(UsersService.GetBanksBrunchsList());


                default:
                    return null;
            }


        }






        //******************************************** End Master Table *****************************

        //******************************************** Report *****************************
        [Authorize]
        [Route("getReportData/{type}")]
        [HttpGet]
        public IHttpActionResult GetReportData(int type)
        {
            return Ok(UsersService.GetReportData(type));
        }


        [Authorize]
        [Route("downloadAllManagerFiles/{Id}/{Shnatmas}")]
        [HttpGet]
        public IHttpActionResult DownloadAllManagerFiles(int Id, int Shnatmas)
        {
            return Ok(UsersService.DownloadAllManagerFiles(Id, Shnatmas));
        }


        [Authorize]
        [Route("importWorkers/{counter}")]
        [HttpPost]
        public IHttpActionResult ImportWorkers(int counter, List<DataModels.Workers> WorkersItems)
        {

            UsersService.ImportWorkers(WorkersItems, counter);
            return Ok();

        }


        [Authorize]
        [Route("getLogsData")]
        [HttpGet]
        public IHttpActionResult GetLogsData(int userid, string start, string end)
        {

            return Ok(UsersService.GetLogsData(userid, start, end));
        }



        [Authorize]
        [Route("getRoles")]
        [HttpGet]
        public IHttpActionResult GetRoles()
        {

            using (var Context = new Context())
            {

                var FarmId = Helper.GetCurrentUser().FarmId;

                var Roles = Context.Roles.Where(x => x.StatusId == 1 && x.Id != 0 && (x.FarmId == null || x.FarmId == FarmId)).ToList();

                return Ok(Roles);


            }


        }
    }
}

using FarmsApi.DataModels;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Http;
using Spire.Pdf;
using System.Drawing.Imaging;
using System;
using System.Drawing;
using Spire.Pdf.Graphics;
using System.Linq;
using FarmsApi.DataModels;
using System.Web;
using iTextSharp.text.xml.xmp;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;
using static System.Net.WebRequestMethods;

namespace FarmsApi.Services
{
    [RoutePrefix("fields")]
    public class FieldsController : ApiController
    {


        [Authorize]
        [Route("actionFieldGroup/{type}/{farmid}/{campainsId}")]
        [HttpPost]
        public IHttpActionResult ActionFieldGroup(int type, int farmid, dynamic objs, int campainsId = -1)
        {

            // שליפה של שדות
            if (type == 1)
            {

                using (var Context = new Context())
                {
                    List<Fields> FieldsList = Context.Fields.Where(x => ((x.FarmId == farmid && x.CampainsId== campainsId) || x.FarmId == null) && x.StatusId == 1).OrderBy(x => x.FarmId).ToList();
                    return Ok(FieldsList);
                }


            }

            // שליפה של קבוצות
            if (type == 2)
            {

                using (var Context = new Context())
                {
                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1 && x.CampainsId == campainsId).OrderBy(x => x.Seq).ToList();


                    return Ok(FieldsGroupsList);
                }

            }


            // שליפה של שדות בתוך הקבוצות
            if (type == 3)
            {

                using (var Context = new Context())
                {


                    var Results = (from f2g in Context.Fields2Groups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId==1 ).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId).DefaultIfEmpty()



                                   select new
                                   {
                                       f2g,
                                       f

                                   }).Where(x=>x.f2g!=null).OrderBy(x => x.f2g.Seq).ToList();


                    return Ok(Results);
                }

            }



            // הוספת שדה
            if (type == 4)
            {

                using (var Context = new Context())
                {



                    string FieldName = objs["Obj"].ToString();

                    Fields FieldsObj = Context.Fields.Where(x => x.FarmId == farmid &&  x.CampainsId == campainsId && x.StatusId == 1 && x.Name == FieldName).FirstOrDefault();


                    if (FieldsObj == null)
                    {
                        Fields f = new Fields();
                        f.Id = 0;
                        f.Name = FieldName;
                        f.FarmId = farmid;
                        f.StatusId = 1;
                        f.CampainsId = campainsId;

                        Context.Fields.Add(f);
                        Context.SaveChanges();

                        List<Fields> FieldsList = Context.Fields.Where(x => ((x.FarmId == farmid && x.CampainsId == campainsId) || x.FarmId == null) && x.StatusId == 1).OrderBy(x => x.FarmId).ToList();
                        return Ok(FieldsList);


                    }
                    else
                    {

                        return Ok("exist obj");

                    }


                    //return Ok(FieldsGroupsList);
                }

            }


            //מחיקה של שדה
            if (type == 5)
            {

                using (var Context = new Context())
                {
                    string FieldId = objs["Obj"].ToString();

                    Fields FieldsObj = Context.Fields.Where(x => x.Id.ToString() == FieldId).FirstOrDefault();
                    FieldsObj.StatusId = 0;
                    Context.Entry(FieldsObj).State = System.Data.Entity.EntityState.Modified;
                    //Context.Fields.Remove(FieldsObj);
                    Context.SaveChanges();

                    List<Fields> FieldsList = Context.Fields.Where(x => ((x.FarmId == farmid && x.CampainsId == campainsId) || x.FarmId == null) && x.StatusId == 1).OrderBy(x => x.FarmId).ToList();
                    return Ok(FieldsList);

                }
            }



            // הוספת קבוצה
            if (type == 6)
            {

                using (var Context = new Context())
                {



                    string GroupName = objs["Obj"].ToString();

                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId == 1).OrderByDescending(x => x.Seq).ToList();


                    //if (FieldsObj == null)
                    //{
                    FieldsGroups fg = new FieldsGroups();
                    fg.Id = 0;
                    fg.Name = GroupName;
                    fg.FarmId = farmid;
                    fg.CampainsId = campainsId;
                    fg.StatusId = 1;
                    fg.CountFieldsInRow = 3;
                    fg.TitleTypeId = 1;

                    if (FieldsGroupsList.Count > 0)
                        fg.Seq = FieldsGroupsList[0].Seq + 1;
                    else
                        fg.Seq = 1;


                    Context.FieldsGroups.Add(fg);
                    Context.SaveChanges();

                    FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
                    return Ok(FieldsGroupsList);


                }

            }



            //מחיקה של קבוצה
            if (type == 7)
            {

                using (var Context = new Context())
                {
                    string FieldsGroupsId = objs["Obj"].ToString();

                    FieldsGroups FieldsGroupsObj = Context.FieldsGroups.Where(x => x.Id.ToString() == FieldsGroupsId).FirstOrDefault();

                    FieldsGroupsObj.StatusId = 0;
                    Context.Entry(FieldsGroupsObj).State = System.Data.Entity.EntityState.Modified;
                    //Context.FieldsGroups.Remove(FieldsGroupsObj);
                    Context.SaveChanges();

                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId == 1).ToList();
                    return Ok(FieldsGroupsList);

                }
            }


            //סדר של הקבוצות
            if (type == 8)
            {



                using (var Context = new Context())
                {


                    List<FieldsGroups> fg = JsonConvert.DeserializeObject<List<FieldsGroups>>(objs.ToString());


                    foreach (var item in fg)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }

                    Context.SaveChanges();


                    var FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
                    return Ok(FieldsGroupsList);

                }
            }

            //הוספה של שדה לקבוצה
            if (type == 9)
            {

                using (var Context = new Context())
                {

                    Fields2Groups f2g = JsonConvert.DeserializeObject<Fields2Groups>(objs.ToString());



                    List<Fields2Groups> Fields2GroupsList = Context.Fields2Groups.Where(x => x.FarmId == farmid && x.FieldsGroupsId == f2g.FieldsGroupsId).OrderByDescending(x => x.Seq).ToList();



                    Fields2Groups f2group = new Fields2Groups();
                    f2group.Id = 0;
                    f2group.FieldsGroupsId = f2g.FieldsGroupsId;
                    f2group.FarmId = farmid;
                    f2group.CampainsId = campainsId;
                    f2group.FieldsId = f2g.FieldsId;
                    f2group.FieldsDataTypesId = 1;
                    f2group.StatusId = 1;

                    if (Fields2GroupsList.Count > 0)
                        f2group.Seq = Fields2GroupsList[0].Seq + 1;
                    else
                        f2group.Seq = 1;


                    Context.Fields2Groups.Add(f2group);
                    Context.SaveChanges();

                    //מחזירים את הזה שלנו החדש
                    return ActionFieldGroup(3, farmid, null,campainsId);

                }
            }

            //מחיקה של שדה בקבוצה
            if (type == 10)
            {

                using (var Context = new Context())
                {
                    string Fields2GroupsId = objs["Obj"].ToString();

                    Fields2Groups Fields2GroupsObj = Context.Fields2Groups.Where(x => x.Id.ToString() == Fields2GroupsId).FirstOrDefault();
                    Fields2GroupsObj.StatusId = 0;
                    Context.Entry(Fields2GroupsObj).State = System.Data.Entity.EntityState.Modified;
                    //Context.Fields2Groups.Remove(Fields2GroupsObj);
                    Context.SaveChanges();

                    //מחזירים את הזה שלנו החדש
                    return ActionFieldGroup(3, farmid, null,campainsId);

                }
            }

            //עריכה בקבוצה
            if (type == 11)
            {

                using (var Context = new Context())
                {


                    FieldsGroups fg = JsonConvert.DeserializeObject<FieldsGroups>(objs.ToString());

                    if (fg != null)
                    {
                        Context.Entry(fg).State = System.Data.Entity.EntityState.Modified;

                        Context.SaveChanges();

                    }

                    var FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.CampainsId == campainsId && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
                    return Ok(FieldsGroupsList);

                }





            }

            //עריכה שדה
            if (type == 12)
            {

                using (var Context = new Context())
                {


                    Fields2Groups f2g = JsonConvert.DeserializeObject<Fields2Groups>(objs.ToString());

                    if (f2g != null)
                    {
                        Context.Entry(f2g).State = System.Data.Entity.EntityState.Modified;

                        Context.SaveChanges();

                    }


                    //מחזירים את הזה שלנו החדש
                    return ActionFieldGroup(3, farmid, null,campainsId);


                }





            }

            //שליפה לפי פידפ שכרגע בוצע שולף את כל הנקודות שעל הפידפ 
            if (type == 13)
            {
                using (var Context = new Context())
                {


                    Fields2PDF fp = JsonConvert.DeserializeObject<Fields2PDF>(objs.ToString());


                    var Results = (from f2p in Context.Fields2PDF.Where(x => x.FarmPDFFilesId == fp.FarmPDFFilesId && x.PageNumber == fp.PageNumber).DefaultIfEmpty()
                                   from f2g in Context.Fields2Groups.Where(x => x.Id == f2p.Fields2GroupsId && x.CampainsId == campainsId).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId || x.Id == f2p.FieldsId).DefaultIfEmpty()


                                   select new
                                   {
                                       f2p,
                                       f

                                   }).ToList();

                    return Ok(Results);
                }

            }

            //עדכון או הוספה או מחיקה לקנבס 
            if (type == 14)
            {
                using (var Context = new Context())
                {

                    Fields2PDF fp = JsonConvert.DeserializeObject<Fields2PDF>(objs.ToString());
                    if (fp != null)
                    {
                        //הוספה
                        if (fp.Id == 0)
                        {
                            Context.Fields2PDF.Add(fp);
                            Context.SaveChanges();
                        }
                        //עדכון
                        else if (fp.StatusId == 1)
                        {
                            Context.Entry(fp).State = System.Data.Entity.EntityState.Modified;
                            Context.SaveChanges();
                        }
                        //מחיקה
                        else
                        {
                            fp = Context.Fields2PDF.Where(x => x.Id == fp.Id).FirstOrDefault();
                            Context.Fields2PDF.Remove(fp);
                            Context.SaveChanges();


                        }
                    }


                    var Results = (from f2p in Context.Fields2PDF.Where(x => x.FarmPDFFilesId == fp.FarmPDFFilesId && x.PageNumber == fp.PageNumber).DefaultIfEmpty()
                                   from f2g in Context.Fields2Groups.Where(x => x.Id == f2p.Fields2GroupsId && x.CampainsId == campainsId).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId || x.Id == f2p.FieldsId).DefaultIfEmpty()


                                   select new
                                   {
                                       f2p,
                                       f

                                   }).ToList();

                    return Ok(Results);

                }

            }

            //שליפה של PDF
            if (type == 15)
            {
                using (var Context = new Context())
                {


                    PdfAPI pa = new PdfAPI();
                    string Link= pa.CreateNewCompanyPDF(farmid, campainsId);



                    return Ok(Link);

                }

            }

            return Ok();
        }


        // [Authorize]
        [Route("getSetWorkerAndCompanyData/{type}/{id}/{campainid}/")]
        [HttpPost]
        public IHttpActionResult GetSetWorkerAndCompanyData(int type, string id, dynamic objs,int campainid=-1)
        {
            string res = id;

            if (Regex.Matches(id, @"[a-zA-Z]").Count > 0)
            {
                id = id.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(id);
            }


            int WorkerId = Convert.ToInt32(res);



            using (var Context = new Context())
            {



                // שליפה של הנתונים
                if (type == 1)
                {

                    var Worker = Context.Workers.Where(x => x.Id == WorkerId).FirstOrDefault();

                    var CurrentFarmId = (Worker != null) ? Worker.FarmId : UsersService.GetCurrentUser().Farm_Id;







                    var Results = (from fields2Groups in Context.Fields2Groups.Where(x => x.FarmId == CurrentFarmId && x.CampainsId== campainid && x.StatusId==1)
                                   from fieldsGroups in Context.FieldsGroups.Where(x => x.Id == fields2Groups.FieldsGroupsId && x.CampainsId == campainid && x.StatusId == 1)

                                   from fields in Context.Fields.Where(x => x.Id == fields2Groups.FieldsId && x.StatusId == 1).DefaultIfEmpty()
                                   from fields2GroupsWorkerData in Context.Fields2GroupsWorkerData.Where(x => x.WorkersId == WorkerId && x.Fields2GroupsId == fields2Groups.Id).DefaultIfEmpty()


                                   select new ResultObjectFields
                                   {
                                       f2g = fields2Groups,
                                       fg = fieldsGroups,
                                       f = fields,
                                       f2gwd = fields2GroupsWorkerData

                                   }).OrderBy(x => x.fg.Seq).ThenBy(x => x.f2g.Seq).ToList();


                    foreach (var item in Results)
                    {
                        if (Worker != null)
                        {

                            if (item.f2gwd == null && item.f.WorkerTableField != null)
                            {

                                var WorkerTableFieldValue = Context.Entry(Worker).Property(item.f.WorkerTableField).CurrentValue;

                                if (WorkerTableFieldValue != null)
                                {
                                    Fields2GroupsWorkerData fields2GroupsWorkerData = new Fields2GroupsWorkerData();
                                    fields2GroupsWorkerData.Fields2GroupsId = item.f2g.Id;
                                    fields2GroupsWorkerData.WorkersId = WorkerId;

                                    if(item.f2g.FieldsDataTypesId==3)
                                        fields2GroupsWorkerData.Value = Convert.ToDateTime(WorkerTableFieldValue).ToString("yyyy-MM-dd");
                                    else 
                                        fields2GroupsWorkerData.Value = WorkerTableFieldValue.ToString();
                                    
                                    fields2GroupsWorkerData.Type = 1;
                                    fields2GroupsWorkerData.SourceValue = WorkerTableFieldValue.ToString();

                                    item.f2gwd = fields2GroupsWorkerData;

                                }

                            }

                            if (item.f2gwd == null && item.f2g.DefaultValue != null)
                            {
                                Fields2GroupsWorkerData fields2GroupsWorkerData = new Fields2GroupsWorkerData();
                                fields2GroupsWorkerData.Fields2GroupsId = item.f2g.Id;
                                fields2GroupsWorkerData.WorkersId = WorkerId;
                                fields2GroupsWorkerData.Value = item.f2g.DefaultValue;
                                fields2GroupsWorkerData.Type = 2;
                                fields2GroupsWorkerData.SourceValue = item.f2g.DefaultValue;

                                item.f2gwd = fields2GroupsWorkerData;

                            }

                        }

                        if (item.f2gwd == null)
                        {
                            Fields2GroupsWorkerData fields2GroupsWorkerData = new Fields2GroupsWorkerData();
                            fields2GroupsWorkerData.Fields2GroupsId = item.f2g.Id;
                            fields2GroupsWorkerData.WorkersId = WorkerId;
                            fields2GroupsWorkerData.Value = null;
                            fields2GroupsWorkerData.Type = 0;
                            fields2GroupsWorkerData.SourceValue = null;

                            item.f2gwd = fields2GroupsWorkerData;

                        }


                    }


                    return Ok(Results);
                }

                // עדכון של הנתונים
                if (type == 2)
                {
                    List<Fields2GroupsWorkerData> f2gwd = JsonConvert.DeserializeObject<List<Fields2GroupsWorkerData>>(objs.ToString());

                    foreach (var item in f2gwd)
                    {
                        

                        //if (item.SourceValue != item.Value)
                        //{
                            if (item.Id == 0)
                            {
                                Context.Fields2GroupsWorkerData.Add(item);
                            }
                            else
                            {
                                Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }


                       // }


                    }

                    Context.SaveChanges();

                    return GetSetWorkerAndCompanyData(1, id, objs, campainid);

                }

                // שליפה של הנתונים
                if (type == 3)
                {
                    var Worker = Context.Workers.Where(x => x.Id == WorkerId).FirstOrDefault();

                    var CurrentFarmId = (Worker != null) ? Worker.FarmId : UsersService.GetCurrentUser().Farm_Id;

                    var Farm = Context.Farms.Where(x => x.Id == CurrentFarmId).FirstOrDefault();

                    return Ok(Farm);

                }

                

            }



            return Ok();
        }

        // [Authorize]
        [Route("getSetCampainsData/{type}/{id}")]
        [HttpPost]
        public IHttpActionResult GetSetCampainsData(int type, string id, dynamic objs)
        {
            string res = id;

            if (Regex.Matches(id, @"[a-zA-Z]").Count > 0)
            {
                id = id.Replace("@@", "+").Replace("ofekslash", "/");
                res = UsersService.DecryptString(id);
            }


            //int WorkerId = Convert.ToInt32(res);



            using (var Context = new Context())
            {


                // שליפה של קמפיין לטובת משתמש שנכנס לקמפיין
                if (type == 5)
                {
                    int CampainId = Convert.ToInt32(res);

                    var Campain = Context.Campains.Where(x => x.Id == CampainId && x.StatusId == 1).FirstOrDefault();

                    return Ok(Campain);

                }


                // שליפה של עובד לטובת משתמש שנכנס לקמפיין
                if (type == 6)
                {
                    int WorkerId = Convert.ToInt32(res);

                    var Workers = Context.Workers.Where(x => x.Id == WorkerId && x.StatusId == 1).FirstOrDefault();

                    return Ok(Workers);

                }



                // שליפה של הנתונים
                if (type == 1)
                {

                    var CampainsList=Context.Campains.Where(x=>x.FarmId.ToString()==id && x.StatusId==1).OrderByDescending(x=>x.DateRigster).ToList();

                    return Ok(CampainsList);
               
                }

                // שליפה של קמפיין לפי איידי
                if (type == 2)
                {

                    var Campain = Context.Campains.Where(x => x.Id.ToString() == id && x.StatusId == 1).FirstOrDefault();

                    return Ok(Campain);
                }

                // שמירה של קמפיין
                if (type == 3)
                {

                    Campains c = JsonConvert.DeserializeObject<Campains>(objs.ToString());

                    if (c != null)
                    {

                        if (c.Id == 0)
                        {

                            Context.Campains.Add(c);


                        }
                        else
                        {
                            Context.Entry(c).State = System.Data.Entity.EntityState.Modified;

                        }


                        Context.SaveChanges();

                    }

                    return Ok(c);
                }


                // שליפה של עובדים תחת קמפיין
                if (type == 4)
                {

                    Campains c = Context.Campains.Where(x => x.Id.ToString() == id && x.StatusId == 1).FirstOrDefault();

                    if (c != null)
                    {

                        var Results = (from w in Context.Workers.Where(x => x.FarmId == c.FarmId && !string.IsNullOrEmpty(x.FirstName) && x.StatusId == 1).DefaultIfEmpty()
                                       from cs in Context.CampainsStatus.Where(x => x.CampainsId == c.Id && x.WorkersId == w.Id).DefaultIfEmpty()
                                       from m in Context.CampainsStatusType.Where(x => x.Id == cs.MediaId && x.Type == 1).DefaultIfEmpty()
                                       from css in Context.CampainsStatusType.Where(x => x.Id == cs.StatusId && x.Type == 2).DefaultIfEmpty()


                                       select new
                                       {
                                           w,
                                           cs,
                                           m, 
                                           css
                                           

                                       }).Where(x=>x.w!=null).ToList();


                        return Ok(Results);

                    }

                    return Ok();
                }




                //// עדכון של הנתונים
                //if (type == 2)
                //{
                //    List<Fields2GroupsWorkerData> f2gwd = JsonConvert.DeserializeObject<List<Fields2GroupsWorkerData>>(objs.ToString());

                //    foreach (var item in f2gwd)
                //    {


                //        //if (item.SourceValue != item.Value)
                //        //{
                //        if (item.Id == 0)
                //        {
                //            Context.Fields2GroupsWorkerData.Add(item);
                //        }
                //        else
                //        {
                //            Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                //        }


                //        // }


                //    }

                //    Context.SaveChanges();

                //    return GetSetWorkerAndCompanyData(1, id, objs);

                //}

                //// שליפה של הנתונים
                //if (type == 3)
                //{
                //    var Worker = Context.Workers.Where(x => x.Id == WorkerId).FirstOrDefault();

                //    var CurrentFarmId = (Worker != null) ? Worker.FarmId : UsersService.GetCurrentUser().Farm_Id;

                //    var Farm = Context.Farms.Where(x => x.Id == CurrentFarmId).FirstOrDefault();

                //    return Ok(Farm);

                //}

            }



            return Ok();
        }

    }
}

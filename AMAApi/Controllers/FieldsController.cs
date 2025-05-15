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

namespace FarmsApi.Services
{
    [RoutePrefix("fields")]
    public class FieldsController : ApiController
    {


        [Authorize]
        [Route("actionFieldGroup/{type}/{farmid}")]
        [HttpPost]
        public IHttpActionResult ActionFieldGroup(int type, int farmid, dynamic objs)
        {

            // שליפה של שדות
            if (type == 1)
            {

                using (var Context = new Context())
                {
                    List<Fields> FieldsList = Context.Fields.Where(x => (x.FarmId == farmid || x.FarmId == null) && x.StatusId == 1).ToList();
                    return Ok(FieldsList);
                }


            }

            // שליפה של קבוצות
            if (type == 2)
            {

                using (var Context = new Context())
                {
                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).OrderBy(x => x.Seq).ToList();


                    return Ok(FieldsGroupsList);
                }

            }


            // שליפה של שדות בתוך הקבוצות
            if (type == 3)
            {

                using (var Context = new Context())
                {


                    var Results = (from f2g in Context.Fields2Groups.Where(x => x.FarmId == farmid).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId).DefaultIfEmpty()



                                   select new
                                   {
                                       f2g,
                                       f

                                   }).OrderBy(x => x.f2g.Seq).ToList();


                    return Ok(Results);
                }

            }



            // הוספת שדה
            if (type == 4)
            {

                using (var Context = new Context())
                {



                    string FieldName = objs["Obj"].ToString();

                    Fields FieldsObj = Context.Fields.Where(x => x.FarmId == farmid && x.StatusId == 1 && x.Name == FieldName).FirstOrDefault();


                    if (FieldsObj == null)
                    {
                        Fields f = new Fields();
                        f.Id = 0;
                        f.Name = FieldName;
                        f.FarmId = farmid;
                        f.StatusId = 1;

                        Context.Fields.Add(f);
                        Context.SaveChanges();

                        List<Fields> FieldsList = Context.Fields.Where(x => (x.FarmId == farmid || x.FarmId == null) && x.StatusId == 1).ToList();
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
                    Context.Fields.Remove(FieldsObj);
                    Context.SaveChanges();

                    List<Fields> FieldsList = Context.Fields.Where(x => (x.FarmId == farmid || x.FarmId == null) && x.StatusId == 1).ToList();
                    return Ok(FieldsList);

                }
            }



            // הוספת קבוצה
            if (type == 6)
            {

                using (var Context = new Context())
                {



                    string GroupName = objs["Obj"].ToString();

                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).OrderByDescending(x => x.Seq).ToList();


                    //if (FieldsObj == null)
                    //{
                    FieldsGroups fg = new FieldsGroups();
                    fg.Id = 0;
                    fg.Name = GroupName;
                    fg.FarmId = farmid;
                    fg.StatusId = 1;
                    fg.CountFieldsInRow = 3;

                    if (FieldsGroupsList.Count > 0)
                        fg.Seq = FieldsGroupsList[0].Seq + 1;
                    else
                        fg.Seq = 1;


                    Context.FieldsGroups.Add(fg);
                    Context.SaveChanges();

                    FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
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
                    Context.FieldsGroups.Remove(FieldsGroupsObj);
                    Context.SaveChanges();

                    List<FieldsGroups> FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).ToList();
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


                    var FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
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
                    f2group.FieldsId = f2g.FieldsId;
                    f2group.FieldsDataTypesId = 1;


                    if (Fields2GroupsList.Count > 0)
                        f2group.Seq = Fields2GroupsList[0].Seq + 1;
                    else
                        f2group.Seq = 1;


                    Context.Fields2Groups.Add(f2group);
                    Context.SaveChanges();

                    //מחזירים את הזה שלנו החדש
                    return ActionFieldGroup(3, farmid, null);

                }
            }

            //מחיקה של שדה בקבוצה
            if (type == 10)
            {

                using (var Context = new Context())
                {
                    string Fields2GroupsId = objs["Obj"].ToString();

                    Fields2Groups Fields2GroupsObj = Context.Fields2Groups.Where(x => x.Id.ToString() == Fields2GroupsId).FirstOrDefault();
                    Context.Fields2Groups.Remove(Fields2GroupsObj);
                    Context.SaveChanges();

                    //מחזירים את הזה שלנו החדש
                    return ActionFieldGroup(3, farmid, null);

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

                    var FieldsGroupsList = Context.FieldsGroups.Where(x => x.FarmId == farmid && x.StatusId == 1).OrderBy(x => x.Seq).ToList();
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
                    return ActionFieldGroup(3, farmid, null);


                }





            }

            //שליפה לפי פידפ שכרגע בוצע שולף את כל הנקודות שעל הפידפ 
            if (type == 13)
            {
                using (var Context = new Context())
                {


                    Fields2PDF fp = JsonConvert.DeserializeObject<Fields2PDF>(objs.ToString());


                    var Results = (from f2p in Context.Fields2PDF.Where(x => x.FarmPDFFilesId == fp.FarmPDFFilesId && x.PageNumber == fp.PageNumber).DefaultIfEmpty()
                                   from f2g in Context.Fields2Groups.Where(x => x.Id == f2p.Fields2GroupsId).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId).DefaultIfEmpty()


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
                        else if(fp.StatusId == 1)
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
                                   from f2g in Context.Fields2Groups.Where(x => x.Id == f2p.Fields2GroupsId).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId).DefaultIfEmpty()


                                   select new
                                   {
                                       f2p,
                                       f

                                   }).ToList();

                    return Ok(Results);

                }

            }

            return Ok();
        }


    }
}

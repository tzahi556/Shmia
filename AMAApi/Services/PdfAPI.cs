﻿using FarmsApi.DataModels;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;
using Image = iTextSharp.text.Image;

namespace FarmsApi.Services
{


    public class PdfAPI
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", ".JPEG", ".JFIF" };

        public string MavidPrati = ConfigurationSettings.AppSettings["MavidPrati"].ToString();
        public string MavidCtovet = ConfigurationSettings.AppSettings["MavidCtovet"].ToString();
        public string MavidPhone = ConfigurationSettings.AppSettings["MavidPhone"].ToString();
        public string MavidNikuyim = ConfigurationSettings.AppSettings["MavidNikuyim"].ToString();

        public int WorkerId = 0;
        public Farm CurrentFarm = new Farm();
        public PdfAPI()
        {


        }


        public string CreateNewCompanyPDF(int FarmId, int CampainsId, WorkersWith101 workersWith101 = null)
        {

            using (var Context = new Context())
            {

                CurrentFarm = Context.Farms.Where(x=>x.Id== FarmId).FirstOrDefault();

                var BaseLinkCompany = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDFS/");
                var BaseLink = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDFS/" + CampainsId + "/");
                var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDFSAllTemplate/");
                var BaseLinkWorker = "";
                if (workersWith101 != null)
                {
                    WorkerId = workersWith101.w.Id;

                    //BaseLink = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDFS/");
                    BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Workers/" + WorkerId.ToString() + "/" + CampainsId.ToString() + "/");
                    BaseLinkWorker = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Workers/" + WorkerId.ToString() + "/");

                }





                if (!Directory.Exists(BaseLinkSite))
                {
                    Directory.CreateDirectory(BaseLinkSite);
                }




                if (workersWith101 == null)
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(BaseLinkSite);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch { }
                    }
                }

                var FarmPDFFilesList = Context.FarmPDFFiles.Where(x => x.FarmId == FarmId && x.StatusId == 1 && x.CampainsId == CampainsId).OrderBy(x => x.Seq).ToList();

                int Counter = 0;

                foreach (var FarmPDFFile in FarmPDFFilesList)
                {

                    Counter++;

                    // if (!FarmPDFFile.Is101) continue;



                    // WorkersWith101 workersWith101 = new WorkersWith101();

                    Document document = new Document();



                    // var existingFile = HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDF/" + FarmPDFFile.FileName);


                    // מעתיק את הטמפלט של כל הפידף
                    string existingFile = System.IO.Path.Combine(BaseLink, FarmPDFFile.FileName);
                    if (FarmPDFFile.Is101)
                    {
                        existingFile = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Companies/101.pdf");

                    }



                    // שם אותו זמני 
                    string newFile = System.IO.Path.Combine(BaseLinkSite, "PDFAllTemplate.pdf");

                    // שם אותו קבוע עם נתונים 
                    string newFileDestination = System.IO.Path.Combine(BaseLinkSite, "PDFAllTemplateEnd_" + Counter + ".pdf");

                    PdfReader reader = new PdfReader(existingFile);

                    //Create a new stream for our output file (this could be a MemoryStream, too)
                    using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        //Use a PdfStamper to bind our source file with our output file
                        using (PdfStamper stamper = new PdfStamper(reader, fs))
                        {
                            //In case of conflict we want our new text to be written "on top" of any existing content
                            //Get the "Over" state for page 1

                            for (int m = 1; m <= reader.NumberOfPages; m++)
                            {
                                PdfContentByte cb = stamper.GetOverContent(m);

                                List<Fields2PDF_101> Fields2PDF_101List = GetFields2PDF_101(FarmPDFFile, m);// Context.Fields2PDF_101.Where(x => x.PageNumber == m).ToList();


                                if (workersWith101 != null)
                                    Fields2PDF_101List = GetDataFromObject101(workersWith101, Fields2PDF_101List, Context);

                                BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
                                foreach (var item in Fields2PDF_101List)
                                {


                                    float llx = (float)item.llx;
                                    float lly = (float)item.lly;
                                    float urx = (float)item.urx;
                                    float ury = (float)item.ury;

                                    if (item.Comment == "SignutureAmuta")
                                    {



                                        Image Signature = Image.GetInstance(BaseLinkCompany + "/SignatureAmuta.png");



                                        Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                        Phrase p = new Phrase();
                                        p.Add(new Chunk(Signature, 0, 0, true));

                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(llx, lly, urx, ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddText(p);

                                        ct.Go();



                                        continue;
                                    }



                                    if (item.Comment == "Signuture")
                                    {


                                        if (!File.Exists(BaseLinkWorker + "/Signature.png")) continue;

                                        Image Signature = Image.GetInstance(BaseLinkWorker + "/Signature.png");



                                        Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                        Phrase p = new Phrase();
                                        p.Add(new Chunk(Signature, 0, 0, true));

                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(llx, lly, urx, ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddText(p);

                                        ct.Go();



                                        continue;
                                    }



                                    int Space = item.Space;
                                    if (Space == 1)
                                    {
                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(llx, lly, urx, ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddElement(new Paragraph(item.Word, font));

                                        ct.Go();
                                    }
                                    else
                                    {
                                        int TextLength = item.Word.Length;

                                        item.Word = Reverse(item.Word);

                                        for (int i = 0; i < TextLength; i++)
                                        {
                                            ColumnText ct = new ColumnText(cb);

                                            ct.SetSimpleColumn(llx, lly, urx - (i * Space), ury);

                                            Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                            ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                            ct.AddElement(new Paragraph(item.Word[i].ToString(), font));

                                            ct.Go();

                                        }

                                    }

                                }

                            }



                        }
                        //fs.Close();
                        //fs.Dispose();

                        //פה מחזיר חזרה
                        File.Copy(newFile, newFileDestination, true);

                        if (File.Exists(newFile))
                        {

                            File.Delete(newFile);

                        }
                    }

                    document.Close();


                }



                string sourceDir = BaseLinkSite;
                string targetPDF = BaseLinkSite + "AllPdfTemp.pdf";
                // string OutputFileDestination = BaseLinkSite + "AllPdf.pdf";



                using (FileStream stream = new FileStream(targetPDF, FileMode.Create))
                {
                    Document pdfDoc = new Document();
                    PdfCopy pdf = new PdfCopy(pdfDoc, stream);
                    pdfDoc.Open();
                    var files = Directory.GetFiles(sourceDir);
                    //Console.WriteLine("Merging files count: " + files.Length);
                    int i = 1;

                    foreach (string file in files)
                    {
                        if (file.ToLower().Contains(".pdf") && !file.Contains("AllPdfTemp"))
                        {
                            PdfReader pdfFile = new PdfReader(file);
                            pdf.AddDocument(pdfFile);
                            pdfFile.Dispose();

                            File.Delete(file);

                        }

                        i++;
                    }


                    if (pdfDoc != null)
                    {


                        if (workersWith101 != null)
                        {



                            sourceDir = BaseLinkSite;
                            files = Directory.GetFiles(sourceDir);


                            var filesImages = files.Where(x => !x.Contains("Signature.png") && ImageExtensions.Contains(System.IO.Path.GetExtension(x).ToUpperInvariant())).ToList();
                            if (filesImages.Count > 0)
                            {
                                var filePath = sourceDir + "/ImagesAll.pdf";
                                CreateImagesPDF(sourceDir, pdf, filesImages);

                            }
                        }

                        pdfDoc.Close();
                        pdfDoc.Dispose();
                    }





                    //File.Copy(targetPDF, OutputFileDestination, true);

                    //if (File.Exists(targetPDF))
                    //{
                    //    File.Delete(targetPDF);
                    //}




                }

                //  return OutputFileDestination;
            }







            return "1";
        }

        private List<Fields2PDF_101> GetFields2PDF_101(FarmPDFFiles FarmPDFFiles, int m)
        {
            using (var Context = new Context())
            {



                List<Fields2PDF_101> Fields2PDF_101List = new List<Fields2PDF_101>();
                if (FarmPDFFiles.Is101)
                {

                    Fields2PDF_101List = Context.Fields2PDF_101.Where(x => x.PageNumber == m).ToList();


                    //var ShnatMas = Fields2PDF_101List.Where(x=>x.Comment== "ShnatMas").FirstOrDefault();

                    return Fields2PDF_101List;
                }

                else
                {
                    var Results = (from f2p in Context.Fields2PDF.Where(x => x.FarmPDFFilesId == FarmPDFFiles.Id && x.PageNumber == m).DefaultIfEmpty()
                                   from f2g in Context.Fields2Groups.Where(x => x.Id == f2p.Fields2GroupsId).DefaultIfEmpty()
                                   from f in Context.Fields.Where(x => x.Id == f2g.FieldsId || x.Id == f2p.FieldsId).DefaultIfEmpty()
                                   from f2gwd in Context.Fields2GroupsWorkerData.Where(x => x.Fields2GroupsId == f2p.Fields2GroupsId && x.WorkersId == WorkerId).DefaultIfEmpty()

                                   select new
                                   {
                                       f2p,
                                       f,
                                       f2g,
                                       f2gwd

                                   }).Where(x => x.f2p != null).ToList();


                    // List<Fields2PDF_101> Fields2PDF_101List=new List<Fields2PDF_101>();  

                    foreach (var item in Results)
                    {
                        Fields2PDF_101 fields2PDF_101 = new Fields2PDF_101();

                        fields2PDF_101.Space = 1;
                        fields2PDF_101.PageNumber = m;
                        fields2PDF_101.Font = 14;
                        fields2PDF_101.Word = item.f.Name;
                        fields2PDF_101.Comment = item.f.Name;


                        if (item.f.WorkerTableField == "1")
                        {
                            fields2PDF_101.Value = "Signuture";
                            fields2PDF_101.Comment = "Signuture";
                            fields2PDF_101.Font = 200;
                            fields2PDF_101.Space = 50;
                        }

                        if (item.f.WorkerTableField == "2")
                        {
                            fields2PDF_101.Value = "SignutureAmuta";
                            fields2PDF_101.Comment = "SignutureAmuta";
                            fields2PDF_101.Font = 100;
                            fields2PDF_101.Space = 40;
                        }


                        fields2PDF_101.llx = 0;
                        fields2PDF_101.urx = item.f2p.PdfWidthX;
                        fields2PDF_101.lly = item.f2p.PdfHeightY - 2;
                        fields2PDF_101.ury = 0;

                        fields2PDF_101.FieldsDataTypesId = (item.f2g != null) ? item.f2g.FieldsDataTypesId : 0;
                        fields2PDF_101.Fields2GroupsWorkerDataValue = (item.f2gwd != null) ? item.f2gwd.Value : null;
                        Fields2PDF_101List.Add(fields2PDF_101);

                    }

                    return Fields2PDF_101List;

                }
            }

        }

        private List<Fields2PDF_101> GetDataFromObject101(WorkersWith101 workersWith101, List<Fields2PDF_101> Fields2PDF_101Lists, Context Context)
        {
            List<Fields2PDF_101> tp = new List<Fields2PDF_101>();

            foreach (var item in Fields2PDF_101Lists)
            {




                var PropertyName = item.Comment;
                var PropertyValue = item.Value;

                if (PropertyName == null) continue;


                if (PropertyValue == "child2") continue;
                if (PropertyValue == "radioInput") continue;
                if (PropertyValue == "Signuture" || PropertyValue == "SignutureAmuta")
                {

                    tp.Add(item);
                    continue;
                }










                var res = workersWith101.w[PropertyName];

                if (res == null && workersWith101.w101!=null)
                {
                    res = workersWith101.w101[PropertyName];
                }


                if (res == null && item.Fields2GroupsWorkerDataValue != null)
                {
                    res = item.Fields2GroupsWorkerDataValue;

                    if (item.FieldsDataTypesId == 3)
                    {
                        res = Convert.ToDateTime(res).ToString("dd/MM/yyyy");
                    }

                    if (item.FieldsDataTypesId == 4 || item.FieldsDataTypesId == 6)
                    {
                        if (res == "true")
                            res = "True";
                        else
                            res = "False";
                    }


                }


                if (PropertyValue == "1Brunch" && res != null)
                {

                    string[] resSplit = res.ToString().Split('-');

                    item.Word = resSplit[0].Trim();
                    tp.Add(item);

                }

                else if (PropertyValue == "2Brunch" && res != null)
                {
                    string[] resSplit = res.ToString().Split('-');

                    if (resSplit.Length > 1)
                        item.Word = resSplit[1].Trim();
                    else
                        item.Word = "";
                    tp.Add(item);

                }

                else if (PropertyValue == "child")
                {

                    var WorkerChilds = Context.WorkerChilds.Where(x => x.WorkerId == workersWith101.w.Id).ToList();

                    for (int i = 0; i < WorkerChilds.Count; i++)
                    {
                        var lly = item.lly - (i * 22);

                        var resChild = WorkerChilds[i][PropertyName];



                        if (PropertyName == "BirthDate" && resChild != null)
                        {
                            resChild = Convert.ToDateTime(resChild).ToString("ddMMyyyy");

                        }




                        Fields2PDF_101 newChild = new Fields2PDF_101();

                        newChild.Word = item.Word;
                        newChild.Value = item.Value;
                        newChild.ury = item.ury;
                        newChild.urx = item.urx;
                        newChild.Space = item.Space;
                        newChild.PageNumber = item.Space;
                        newChild.lly = lly;
                        newChild.llx = item.llx;
                        newChild.Id = item.Id;
                        newChild.Font = item.Font;
                        newChild.Comment = item.Comment;





                        if (item.Word.Contains("x") && resChild.ToString() == "True")
                            newChild.Word = "x";
                        else
                          if (resChild.ToString() == "False") { newChild.Word = ""; } else if (resChild != null) { newChild.Word = resChild.ToString(); }



                        tp.Add(newChild);




                    }



                    // item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    //  tp.Add(item);

                }

                else if (res != null && PropertyName == "TiumMasAnotherMaskuretSug")
                {

                    item.Word = GetSugAnother(res.ToString());
                    tp.Add(item);

                }

                else if (PropertyName == "CurrentDate")
                {

                    item.Word = DateTime.Now.ToString("dd.MM.yyyy");
                    tp.Add(item);

                }

                else if (PropertyName == "MavidPrati")
                {

                    item.Word = CurrentFarm.Name; //MavidPrati;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidCtovet")
                {

                    item.Word = CurrentFarm.Address; // MavidCtovet;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidPhone")
                {

                    item.Word = CurrentFarm.OfficeNumber; //MavidPhone;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidNikuyim")
                {

                    item.Word = CurrentFarm.TikNikuim; //MavidNikuyim;
                    tp.Add(item);

                }

                else if (res != null && PropertyName == "Kupa")
                {

                    item.Word = GetKupaName(res.ToString());
                    tp.Add(item);

                }

                else if (res != null && PropertyValue == "Date")
                {
                    item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    tp.Add(item);

                }

                else if (res != null && PropertyValue == "DateDot")
                {
                    item.Word = Convert.ToDateTime(res).ToString("dd.MM.yyyy");
                    tp.Add(item);

                }

                else if (res != null && ((PropertyValue == null) || PropertyValue == res.ToString() || res.ToString() == "True") && res.ToString() != "False")
                {
                    if (item.Word.Contains("x"))
                        item.Word = "x";
                    else
                        item.Word = res.ToString();

                    tp.Add(item);
                }
            }




            return tp;
        }






        private List<Fields2PDF_101> GetDataFromObject_GenericData(WorkersWith101 workersWith101, List<Fields2PDF_101> Fields2PDF_101Lists, Context Context)
        {
            List<Fields2PDF_101> tp = new List<Fields2PDF_101>();

            foreach (var item in Fields2PDF_101Lists)
            {




                var PropertyName = item.Comment;
                var PropertyValue = item.Value;

                if (PropertyName == null) continue;


                if (PropertyValue == "child2") continue;
                if (PropertyValue == "radioInput") continue;
                if (PropertyValue == "Signuture" || PropertyValue == "SignutureAmuta")
                {

                    tp.Add(item);
                    continue;
                }










                var res = workersWith101.w[PropertyName];

                if (res == null)
                {
                    res = workersWith101.w101[PropertyName];
                }



                if (PropertyValue == "1Brunch" && res != null)
                {

                    string[] resSplit = res.ToString().Split('-');

                    item.Word = resSplit[0].Trim();
                    tp.Add(item);

                }

                else if (PropertyValue == "2Brunch" && res != null)
                {
                    string[] resSplit = res.ToString().Split('-');

                    if (resSplit.Length > 1)
                        item.Word = resSplit[1].Trim();
                    else
                        item.Word = "";
                    tp.Add(item);

                }

                else if (PropertyValue == "child")
                {

                    var WorkerChilds = Context.WorkerChilds.Where(x => x.WorkerId == workersWith101.w.Id).ToList();

                    for (int i = 0; i < WorkerChilds.Count; i++)
                    {
                        var lly = item.lly - (i * 22);

                        var resChild = WorkerChilds[i][PropertyName];



                        if (PropertyName == "BirthDate" && resChild != null)
                        {
                            resChild = Convert.ToDateTime(resChild).ToString("ddMMyyyy");

                        }




                        Fields2PDF_101 newChild = new Fields2PDF_101();

                        newChild.Word = item.Word;
                        newChild.Value = item.Value;
                        newChild.ury = item.ury;
                        newChild.urx = item.urx;
                        newChild.Space = item.Space;
                        newChild.PageNumber = item.Space;
                        newChild.lly = lly;
                        newChild.llx = item.llx;
                        newChild.Id = item.Id;
                        newChild.Font = item.Font;
                        newChild.Comment = item.Comment;





                        if (item.Word.Contains("x") && resChild.ToString() == "True")
                            newChild.Word = "x";
                        else
                          if (resChild.ToString() == "False") { newChild.Word = ""; } else if (resChild != null) { newChild.Word = resChild.ToString(); }



                        tp.Add(newChild);




                    }



                    // item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    //  tp.Add(item);

                }

                else if (res != null && PropertyName == "TiumMasAnotherMaskuretSug")
                {

                    item.Word = GetSugAnother(res.ToString());
                    tp.Add(item);

                }

                else if (PropertyName == "CurrentDate")
                {

                    item.Word = DateTime.Now.ToString("dd.MM.yyyy");
                    tp.Add(item);

                }

                else if (PropertyName == "MavidPrati")
                {

                    item.Word = MavidPrati;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidCtovet")
                {

                    item.Word = MavidCtovet;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidPhone")
                {

                    item.Word = MavidPhone;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidNikuyim")
                {

                    item.Word = MavidNikuyim;
                    tp.Add(item);

                }

                else if (res != null && PropertyName == "Kupa")
                {

                    item.Word = GetKupaName(res.ToString());
                    tp.Add(item);

                }

                else if (res != null && PropertyValue == "Date")
                {
                    item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    tp.Add(item);

                }

                else if (res != null && PropertyValue == "DateDot")
                {
                    item.Word = Convert.ToDateTime(res).ToString("dd.MM.yyyy");
                    tp.Add(item);

                }

                else if (res != null && ((PropertyValue == null) || PropertyValue == res.ToString() || res.ToString() == "True") && res.ToString() != "False")
                {
                    if (item.Word.Contains("x"))
                        item.Word = "x";
                    else
                        item.Word = res.ToString();

                    tp.Add(item);
                }
            }


            //foreach (var property in typeof(Workers).GetProperties())
            //{

            //    var res = w.GetType().GetProperty(property.Name).GetValue(w, null);

            //}

            return tp;
        }


        public int CreatePDF(WorkersWith101 workersWith101)
        {


            //    System.Threading.Thread.Sleep(5000);

            Document document = new Document();

            var BaseLink = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + workersWith101.w.Id);

            if (!Directory.Exists(BaseLinkSite))
            {
                Directory.CreateDirectory(BaseLinkSite);
            }

            // מעתיק את הטמפלט של כל הפידף
            string existingFile = System.IO.Path.Combine(BaseLink, "OfekAll.pdf");

            // שם אותו זמני 
            string newFile = System.IO.Path.Combine(BaseLinkSite, "OfekAllTemp.pdf");

            // שם אותו קבוע עם נתונים 
            string newFileDestination = System.IO.Path.Combine(BaseLinkSite, "OfekAll.pdf");


            PdfReader reader = new PdfReader(existingFile);


            //Create a new stream for our output file (this could be a MemoryStream, too)
            using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                //Use a PdfStamper to bind our source file with our output file
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    //In case of conflict we want our new text to be written "on top" of any existing content
                    //Get the "Over" state for page 1
                    using (var Context = new Context())
                    {
                        for (int m = 1; m < 10; m++)
                        {
                            PdfContentByte cb = stamper.GetOverContent(m);
                            var TestList = Context.Testpdfs.Where(x => x.PageNumber == m).ToList();


                            TestList = GetDataFromObject(workersWith101, TestList, Context);

                            BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
                            foreach (var item in TestList)
                            {


                                if (item.Comment == "SignutureAmuta")
                                {

                                    Image Signature = Image.GetInstance(BaseLink + "/SignatureAmuta.png");



                                    Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                    Phrase p = new Phrase();
                                    p.Add(new Chunk(Signature, 0, 0, true));

                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddText(p);

                                    ct.Go();



                                    continue;
                                }



                                if (item.Comment == "Signuture")
                                {


                                    if (!File.Exists(BaseLinkSite + "/Signature.png")) continue;

                                    Image Signature = Image.GetInstance(BaseLinkSite + "/Signature.png");
                                    //logo.ScaleAbsolute(500, 300);


                                    Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                    Phrase p = new Phrase();
                                    p.Add(new Chunk(Signature, 0, 0, true));

                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddText(p);

                                    ct.Go();



                                    continue;
                                }



                                int Space = item.Space;
                                if (Space == 1)
                                {
                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddElement(new Paragraph(item.Word, font));

                                    ct.Go();
                                }
                                else
                                {
                                    int TextLength = item.Word.Length;

                                    item.Word = Reverse(item.Word);

                                    for (int i = 0; i < TextLength; i++)
                                    {
                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(item.llx, item.lly, item.urx - (i * Space), item.ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddElement(new Paragraph(item.Word[i].ToString(), font));

                                        ct.Go();

                                    }

                                }

                            }

                        }
                    }


                }
                //fs.Close();
                //fs.Dispose();

                //פה מחזיר חזרה
                File.Copy(newFile, newFileDestination, true);


                if (File.Exists(newFile))
                {

                    File.Delete(newFile);

                }
            }

            document.Close();


            //מעתיק את כל המסמכים הפידףים המצורפים
            var BaseLinkSite2 = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + workersWith101.w.Id);
            string sourceDir = BaseLinkSite2;
            string OutputFile = BaseLinkSite2 + "/OfekAllPdfTemp.pdf";
            string OutputFileDestination = BaseLinkSite2 + "/OfekAllPdf.pdf";

            //בפנים יש צירוף של תמונות 
            CreateMergedPDF(OutputFile, sourceDir, OutputFileDestination);


            return 1;

        }

        public int CreatePDFOnly101(WorkersWith101 workersWith101)
        {


            //    System.Threading.Thread.Sleep(5000);

            Document document = new Document();




            var BaseLink = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            var BaseLinkSite = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + workersWith101.w.Id);

            if (!Directory.Exists(BaseLinkSite))
            {
                Directory.CreateDirectory(BaseLinkSite);
            }

            // מעתיק את הטמפלט של כל הפידף
            string existingFile = System.IO.Path.Combine(BaseLink, "101.pdf");

            // שם אותו זמני 
            string newFile = System.IO.Path.Combine(BaseLinkSite, "OfekAllTemp.pdf");

            // שם אותו קבוע עם נתונים 
            string newFileDestination = System.IO.Path.Combine(BaseLinkSite, workersWith101.w101.UniqNumber + ".pdf");
            // string newFileDestination = "";


            PdfReader reader = new PdfReader(existingFile);


            //Create a new stream for our output file (this could be a MemoryStream, too)
            using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                //Use a PdfStamper to bind our source file with our output file
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    //In case of conflict we want our new text to be written "on top" of any existing content
                    //Get the "Over" state for page 1
                    using (var Context = new Context())
                    {
                        for (int m = 1; m < 3; m++)
                        {
                            PdfContentByte cb = stamper.GetOverContent(m);
                            var TestList = Context.Testpdfs.Where(x => x.PageNumber == m + 1).ToList();

                            TestList = GetDataFromObject(workersWith101, TestList, Context);

                            BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
                            foreach (var item in TestList)
                            {


                                if (item.Comment == "SignutureAmuta")
                                {

                                    Image Signature = Image.GetInstance(BaseLink + "/SignatureAmuta.png");



                                    Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                    Phrase p = new Phrase();
                                    p.Add(new Chunk(Signature, 0, 0, true));

                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddText(p);

                                    ct.Go();



                                    continue;
                                }



                                if (item.Comment == "Signuture")
                                {


                                    if (!File.Exists(BaseLinkSite + "/Signature.png")) continue;

                                    Image Signature = Image.GetInstance(BaseLinkSite + "/Signature.png");
                                    //logo.ScaleAbsolute(500, 300);


                                    Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                    Phrase p = new Phrase();
                                    p.Add(new Chunk(Signature, 0, 0, true));

                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddText(p);

                                    ct.Go();



                                    continue;
                                }



                                int Space = item.Space;
                                if (Space == 1)
                                {
                                    ColumnText ct = new ColumnText(cb);

                                    ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                    Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                    ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                    ct.AddElement(new Paragraph(item.Word, font));

                                    ct.Go();
                                }
                                else
                                {
                                    int TextLength = item.Word.Length;

                                    item.Word = Reverse(item.Word);

                                    for (int i = 0; i < TextLength; i++)
                                    {
                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(item.llx, item.lly, item.urx - (i * Space), item.ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddElement(new Paragraph(item.Word[i].ToString(), font));

                                        ct.Go();

                                    }

                                }

                            }

                        }
                    }


                }
                //fs.Close();
                //fs.Dispose();

                //פה מחזיר חזרה
                File.Copy(newFile, newFileDestination, true);


                if (File.Exists(newFile))
                {

                    File.Delete(newFile);

                }
            }








            document.Close();


            //מעתיק את כל המסמכים הפידףים המצורפים
            //var BaseLinkSite2 = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + w.Id);
            //string sourceDir = BaseLinkSite2;
            //string OutputFile = BaseLinkSite2 + "/OfekAllPdfTemp.pdf";
            //string OutputFileDestination = BaseLinkSite2 + "/OfekAllPdf.pdf";

            ////בפנים יש צירוף של תמונות 
            //CreateMergedPDF(OutputFile, sourceDir, OutputFileDestination);


            return 1;

        }



        private List<Testpdfs> GetDataFromObject(WorkersWith101 workersWith101, List<Testpdfs> TestLists, Context Context)
        {
            List<Testpdfs> tp = new List<Testpdfs>();

            foreach (var item in TestLists)
            {
                var PropertyName = item.Comment;
                var PropertyValue = item.Value;

                if (PropertyName == null) continue;


                if (PropertyValue == "child2") continue;
                if (PropertyValue == "radioInput") continue;
                if (PropertyValue == "Signuture" || PropertyValue == "SignutureAmuta")
                {

                    tp.Add(item);
                    continue;
                }










                var res = workersWith101.w[PropertyName];

                if (res == null)
                {

                    res = workersWith101.w101[PropertyName];


                }



                if (PropertyValue == "1Brunch" && res != null)
                {

                    string[] resSplit = res.ToString().Split('-');

                    item.Word = resSplit[0].Trim();
                    tp.Add(item);

                }



                else if (PropertyValue == "2Brunch" && res != null)
                {
                    string[] resSplit = res.ToString().Split('-');

                    if (resSplit.Length > 1)
                        item.Word = resSplit[1].Trim();
                    else
                        item.Word = "";
                    tp.Add(item);

                }



                else if (PropertyValue == "child")
                {

                    var WorkerChilds = Context.WorkerChilds.Where(x => x.WorkerId == workersWith101.w.Id).ToList();

                    for (int i = 0; i < WorkerChilds.Count; i++)
                    {
                        var lly = item.lly - (i * 22);

                        var resChild = WorkerChilds[i][PropertyName];



                        if (PropertyName == "BirthDate" && resChild != null)
                        {
                            resChild = Convert.ToDateTime(resChild).ToString("ddMMyyyy");

                        }




                        Testpdfs newChild = new Testpdfs();

                        newChild.Word = item.Word;
                        newChild.Value = item.Value;
                        newChild.ury = item.ury;
                        newChild.urx = item.urx;
                        newChild.Space = item.Space;
                        newChild.PageNumber = item.Space;
                        newChild.lly = lly;
                        newChild.llx = item.llx;
                        newChild.Id = item.Id;
                        newChild.Font = item.Font;
                        newChild.Comment = item.Comment;





                        if (item.Word.Contains("x") && resChild.ToString() == "True")
                            newChild.Word = "x";
                        else
                          if (resChild.ToString() == "False") { newChild.Word = ""; } else if (resChild != null) { newChild.Word = resChild.ToString(); }



                        tp.Add(newChild);




                    }



                    // item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    //  tp.Add(item);

                }






                else if (res != null && PropertyName == "TiumMasAnotherMaskuretSug")
                {

                    item.Word = GetSugAnother(res.ToString());
                    tp.Add(item);

                }

                else if (PropertyName == "CurrentDate")
                {

                    item.Word = DateTime.Now.ToString("dd.MM.yyyy");
                    tp.Add(item);

                }


                else if (PropertyName == "MavidPrati")
                {

                    item.Word = MavidPrati;
                    tp.Add(item);

                }


                else if (PropertyName == "MavidCtovet")
                {

                    item.Word = MavidCtovet;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidPhone")
                {

                    item.Word = MavidPhone;
                    tp.Add(item);

                }

                else if (PropertyName == "MavidNikuyim")
                {

                    item.Word = MavidNikuyim;
                    tp.Add(item);

                }



                else if (res != null && PropertyName == "Kupa")
                {

                    item.Word = GetKupaName(res.ToString());
                    tp.Add(item);

                }


                else if (res != null && PropertyValue == "Date")
                {
                    item.Word = Convert.ToDateTime(res).ToString("ddMMyyyy");
                    tp.Add(item);

                }

                else if (res != null && PropertyValue == "DateDot")
                {
                    item.Word = Convert.ToDateTime(res).ToString("dd.MM.yyyy");
                    tp.Add(item);

                }

                else if (res != null && ((PropertyValue == null) || PropertyValue == res.ToString() || res.ToString() == "True") && res.ToString() != "False")
                {
                    if (item.Word.Contains("x"))
                        item.Word = "x";
                    else
                        item.Word = res.ToString();

                    tp.Add(item);
                }
            }


            //foreach (var property in typeof(Workers).GetProperties())
            //{

            //    var res = w.GetType().GetProperty(property.Name).GetValue(w, null);

            //}

            return tp;
        }

        private string GetKupaName(string propertyName)
        {
            switch (propertyName)
            {
                case "1":
                    return "כללית";

                case "2":
                    return "מכבי";

                case "3":
                    return "מאוחדת";

                case "4":
                    return "לאומית";



                default:
                    return "";

            }
        }

        private string GetSugAnother(string propertyName)
        {
            switch (propertyName)
            {
                case "1":
                    return "עבודה";

                case "2":
                    return "קיצבה";

                case "3":
                    return "מילגה";

                case "4":
                    return "אחר";



                default:
                    return "";

            }
        }

        public void GetEnumerator(Workers w)
        {
            foreach (var property in typeof(Workers).GetProperties())
            {

                var res = w.GetType().GetProperty(property.Name).GetValue(w, null);
                //var res = w[property];
                //yield return property;
            }
        }

        void CreateMergedPDF(string targetPDF, string sourceDir, string OutputFileDestination)
        {
            using (FileStream stream = new FileStream(targetPDF, FileMode.Create))
            {
                Document pdfDoc = new Document();
                PdfCopy pdf = new PdfCopy(pdfDoc, stream);
                pdfDoc.Open();
                var files = Directory.GetFiles(sourceDir);
                //Console.WriteLine("Merging files count: " + files.Length);
                int i = 1;

                //  pdf.AddDocument(new PdfReader(sourceDir + "/OfekAll.pdf"));


                PdfReader newReader = new PdfReader(sourceDir + "/OfekAll.pdf");
                pdf.AddDocument(newReader);
                newReader.Close();
                newReader.Dispose();






                foreach (string file in files)
                {
                    if (file.ToLower().Contains(".pdf") && !file.Contains("OfekAll"))
                    {
                        //  Console.WriteLine(i + ". Adding: " + file);
                        pdf.AddDocument(new PdfReader(file));

                    }

                    i++;
                }





                var filesImages = files.Where(x => !x.Contains("Signature.png") && ImageExtensions.Contains(System.IO.Path.GetExtension(x).ToUpperInvariant())).ToList();
                if (filesImages.Count > 0)
                {
                    var filePath = sourceDir + "/ImagesAll.pdf";
                    CreateImagesPDF(sourceDir, pdf, filesImages);

                }

                if (pdfDoc != null)
                {

                    pdfDoc.Close();
                    pdfDoc.Dispose();
                }


                File.Copy(targetPDF, OutputFileDestination, true);

                if (File.Exists(targetPDF))
                {
                    File.Delete(targetPDF);
                }

            }
        }



        private void CreateImagesPDF(string sourceDir, PdfCopy pdf, List<string> filesImages)
        {
            string targetPDF = sourceDir + "/ImagesAll.pdf";


            using (FileStream fs = new FileStream(targetPDF, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            {

                Document document = new Document();
                // Create an instance to the PDF file by creating an instance of the PDF   
                // Writer class using the document and the filestrem in the constructor.  
                //  PdfCopy pdf = new PdfCopy(document, fs);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                // Open the document to enable you to write to the document  
                document.Open();

                // var files = Directory.GetFiles(sourceDir);


                foreach (var item in filesImages)
                {
                    Paragraph paragraph = new Paragraph();



                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(item);

                    //Resize image depend upon your need
                    // float pageWidth = document.PageSize.Width - (35 + 35);
                    jpg.ScaleToFit(document.PageSize.Width - 30, document.PageSize.Height - 30);

                    //Give space before image

                    jpg.SpacingBefore = 10f;

                    //Give some space after the image

                    jpg.SpacingAfter = 1f;

                    jpg.Alignment = Element.ALIGN_CENTER;



                    document.Add(paragraph);

                    document.Add(jpg);

                }




                //// Add a simple and wellknown phrase to the document in a flow layout manner  
                //document.Add(new Paragraph("Hello World!"));
                // Close the document  
                document.Close();
                // Close the writer instance  
                writer.Close();

                //if (document != null)
                //    document.Close();
                // Always close open filehandles explicity  
                //  
                fs.Close();
                fs.Dispose();

                PdfReader newReader = new PdfReader(targetPDF);
                pdf.AddDocument(newReader);
                newReader.Close();
                newReader.Dispose();

                if (File.Exists(targetPDF))
                {
                    File.Delete(targetPDF);

                }


                // 
            }
        }

        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>


        public void TestPdfNewFromDB(int llx, int lly, int urx, int ury, string text)
        {



            // step 1: creation of a document-object
            Document document = new Document();


            try
            {

                var BaseLink = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
                var BaseLinkSite = @"C:\Dev\Shmia\site\";

                string existingFile = System.IO.Path.Combine(BaseLink, "OfekAll.pdf");

                string newFile = System.IO.Path.Combine(BaseLinkSite, "102.pdf");

                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }


                PdfReader reader = new PdfReader(existingFile);


                //Create a new stream for our output file (this could be a MemoryStream, too)
                using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //Use a PdfStamper to bind our source file with our output file
                    using (PdfStamper stamper = new PdfStamper(reader, fs))
                    {



                        //In case of conflict we want our new text to be written "on top" of any existing content
                        //Get the "Over" state for page 1



                        using (var Context = new Context())
                        {


                            for (int m = 1; m < 10; m++)
                            {
                                PdfContentByte cb = stamper.GetOverContent(m);
                                var TestList = Context.Testpdfs.Where(x => x.PageNumber == m).ToList();
                                BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
                                foreach (var item in TestList)
                                {
                                    if (item.Comment == "Signuture" || item.Comment == "SignutureAmuta")
                                    {


                                        Image Signature = Image.GetInstance(BaseLink + "/SignatureAmuta.png");
                                        Signature.ScaleAbsolute(float.Parse(((int)item.Font).ToString()), float.Parse(((int)item.Space).ToString()));


                                        //   Signature.ScaleAbsolute(new Rectangle(item.llx, item.lly, item.urx, item.ury));

                                        Phrase p = new Phrase();
                                        p.Add(new Chunk(Signature, 0, 0, true));

                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddText(p);

                                        ct.Go();

                                        continue;

                                    }


                                    int Space = item.Space;

                                    if (Space == 1)
                                    {
                                        ColumnText ct = new ColumnText(cb);

                                        ct.SetSimpleColumn(item.llx, item.lly, item.urx, item.ury);

                                        Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                        ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                        ct.AddElement(new Paragraph(item.Word, font));

                                        ct.Go();
                                    }
                                    else
                                    {
                                        int TextLength = item.Word.Length;

                                        item.Word = Reverse(item.Word);

                                        for (int i = 0; i < TextLength; i++)
                                        {
                                            ColumnText ct = new ColumnText(cb);

                                            ct.SetSimpleColumn(item.llx, item.lly, item.urx - (i * Space), item.ury);

                                            Font font = new Font(bf, float.Parse(item.Font.ToString()));

                                            ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                                            ct.AddElement(new Paragraph(item.Word[i].ToString(), font));

                                            ct.Go();

                                        }

                                    }

                                }

                            }
                        }








                    }
                }





            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }


            // step 5: we close the document
            document.Close();

        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //public void TestPdfNew(int llx, int lly, int urx, int ury, string text)
        //{


        //    // step 1: creation of a document-object
        //    Document document = new Document();


        //    try
        //    {

        //        var BaseLink = System.Web.HttpContext.Current.Server.MapPath(@"/App_Data/");
        //        var BaseLinkSite = @"C:\Dev\Ofek\site\";

        //        string existingFile = System.IO.Path.Combine(BaseLink, "101.pdf");

        //        string newFile = System.IO.Path.Combine(BaseLinkSite, "102.pdf");

        //        if (File.Exists(newFile))
        //        {
        //            File.Delete(newFile);
        //        }


        //        PdfReader reader = new PdfReader(existingFile);


        //        //Create a new stream for our output file (this could be a MemoryStream, too)
        //        using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
        //        {
        //            //Use a PdfStamper to bind our source file with our output file
        //            using (PdfStamper stamper = new PdfStamper(reader, fs))
        //            {



        //                //In case of conflict we want our new text to be written "on top" of any existing content
        //                //Get the "Over" state for page 1
        //                PdfContentByte cb = stamper.GetOverContent(1);





        //                ColumnText ct = new ColumnText(cb);

        //                ////Create a single column who's lower left corner is at 100x100 and upper right is at 500x200
        //                ct.SetSimpleColumn(llx, lly, urx, ury);


        //                BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
        //                Font font = new Font(bf, 14);

        //                ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;



        //                //String text2 = "<img src='C:/Dev/Ofek/site/images/klalitPlatinum_.png' />";
        //                //List<IElement> parsedText = ConvertToHtmlForColumnText(text2);

        //                //ct.Add(parsedText);

        //                //Add a higher level object
        //                ct.AddElement(new Paragraph(text, font));


        //                //var rect = new iTextSharp.text.Rectangle(llx, lly, urx, ury);
        //                //rect.Border = iTextSharp.text.Rectangle.BOX;
        //                //rect.BorderWidth = 5;
        //                //rect.BorderColor = BaseColor.PINK;
        //                //cb.Rectangle(rect);




        //                //Flush the text buffer
        //                ct.Go();

        //            }
        //        }





        //    }
        //    catch (DocumentException de)
        //    {
        //        Console.Error.WriteLine(de.Message);
        //    }
        //    catch (IOException ioe)
        //    {
        //        Console.Error.WriteLine(ioe.Message);
        //    }


        //    // step 5: we close the document
        //    document.Close();


        //}

        public List<IElement> ConvertToHtmlForColumnText(String text)
        {
            ListElementHandler listHandler = new ListElementHandler();
            XMLWorkerHelper.GetInstance().ParseXHtml(listHandler, new StringReader(text));
            return listHandler.List;
        }

        //public void TestPdf()
        //{


        //    // step 1: creation of a document-object
        //    Document document = new Document();


        //    try
        //    {

        //        var BaseLink = System.Web.HttpContext.Current.Server.MapPath(@"/App_Data/");
        //        var BaseLinkSite = @"C:\Dev\Ofek\site\";

        //        string existingFile = System.IO.Path.Combine(BaseLink, "101.pdf");

        //        string newFile = System.IO.Path.Combine(BaseLinkSite, "102.pdf");

        //        if (File.Exists(newFile))
        //        {
        //            File.Delete(newFile);
        //        }

        //        //using (FileStream fs = new FileStream(existingFile, FileMode.Create, FileAccess.Write, FileShare.None))
        //        //{
        //        //    using (Document doc = new Document(PageSize.LETTER))
        //        //    {
        //        //        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
        //        //        {
        //        //            doc.Open();

        //        //            doc.Add(new Paragraph("This is a test"));

        //        //            doc.Close();
        //        //        }
        //        //    }
        //        //}

        //        //Bind a PdfReader to our first document
        //        PdfReader reader = new PdfReader(existingFile);




        //        //for (int page = 1; page <= reader.NumberOfPages; page++)
        //        //{
        //        //    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

        //        //    string currentPageText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
        //        //    if (currentPageText.Contains("דבוע סיטרכ"))
        //        //    {

        //        //        //pages.Add(page);
        //        //    }
        //        //}













        //        //Create a new stream for our output file (this could be a MemoryStream, too)
        //        using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
        //        {
        //            //Use a PdfStamper to bind our source file with our output file
        //            using (PdfStamper stamper = new PdfStamper(reader, fs))
        //            {









        //                //In case of conflict we want our new text to be written "on top" of any existing content
        //                //Get the "Over" state for page 1
        //                PdfContentByte cb = stamper.GetOverContent(1);

        //                //Begin text command
        //                cb.BeginText();
        //                //Set the font information
        //                cb.SetFontAndSize(BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true), 16f);
        //                //Position the cursor for drawing
        //                cb.MoveText(50, 50);


        //                // 290.6475x742.3978
        //                //Write some text
        //                cb.ShowText("מה קורה???");
        //                //End text command
        //                cb.EndText();

        //                //Create a new ColumnText object to write to
        //                ColumnText ct = new ColumnText(cb);
        //                //Create a single column who's lower left corner is at 100x100 and upper right is at 500x200
        //                ct.SetSimpleColumn(500, 200, 100, 100);
        //                //290.6475F, 742.3978F, 347.8235F, 756.3978F
        //                //llx:290.6475 urx:347.8235 ury:756.3978 lly:742.3978
        //                //Left:290.6475 Right:347.8235 Top:756.3978 Bootom:742.3978

        //                BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, true);
        //                Font font = new Font(bf, 14);

        //                ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
        //                //Add a higher level object
        //                ct.AddElement(new Paragraph("מה קורה?", font));


        //                var rect = new iTextSharp.text.Rectangle(500, 200);
        //                rect.Border = iTextSharp.text.Rectangle.BOX;
        //                rect.BorderWidth = 5;
        //                rect.BorderColor = new BaseColor(0, 0, 0);
        //                cb.Rectangle(rect);

        //                rect = new iTextSharp.text.Rectangle(200, 200, 100, 400);
        //                rect.Border = iTextSharp.text.Rectangle.BOX;
        //                rect.BorderWidth = 5;
        //                rect.BorderColor = BaseColor.PINK;
        //                cb.Rectangle(rect);



        //                //Flush the text buffer
        //                ct.Go();

        //            }
        //        }





        //    }
        //    catch (DocumentException de)
        //    {
        //        Console.Error.WriteLine(de.Message);
        //    }
        //    catch (IOException ioe)
        //    {
        //        Console.Error.WriteLine(ioe.Message);
        //    }


        //    // step 5: we close the document
        //    document.Close();


        //}




    }

    public static class ColumnTextListExtension
    {
        public static void Add(this ColumnText ct, List<IElement> elements)
        {
            foreach (IElement e in elements)
            {

                ct.AddElement(e);
            }
        }
    }

    public class ListElementHandler : IElementHandler
    {
        List<IElement> elements = new List<IElement>();

        public List<IElement> List => elements;

        public void Add(IWritable w)
        {
            if (w is WritableElement)
            {
                foreach (IElement e in ((WritableElement)w).Elements())
                {
                    elements.Add(e);
                }
            }
        }
    }



}




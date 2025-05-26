using FarmsApi.Services;
using Google.Cloud.Vision.V1;
using Grpc.Core;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
//using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FarmsApi.DataModels;
using System.Security.Cryptography;
using Amazon.Auth.AccessControlPolicy;


namespace FarmsApi.Controllers
{
    [RoutePrefix("files")]
    public class FilesController : ApiController
    {
        [Route("upload/{folder}/{workerid}")]
        [HttpPost]
        public async Task<IHttpActionResult> Upload(string folder, int workerid)
        {

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);


            string root = HttpContext.Current.Server.MapPath("~/Uploads/");

            if (folder.Contains("Companies"))
            {
                root = HttpContext.Current.Server.MapPath("~/Uploads/Companies/");
            }



            string tempRoot = root;

            string WorkerPath = root + workerid.ToString();

            if (!Directory.Exists(WorkerPath))
            {
                Directory.CreateDirectory(WorkerPath);

            }

            root = WorkerPath;

            var provider = new MultipartFormDataStreamProvider(root);

            var file = await Request.Content.ReadAsMultipartAsync(provider);

            if (folder.Contains("taz_"))
            {
                //string credential_path = tempRoot + "CrediJson/cerdi.json";   //HttpContext.Current.Server.MapPath("~/Uploads/CrediJson/cerdi.json");
                ////// יצירת סרביס אקאונט ואז יצירת מפתח ולהוריד את הגייסון
                ///// כשלא עובד להלן הסבר עם לינק פשוט ליצור אחד חדש
                /////https://stackoverflow.com/questions/72198894/how-to-download-the-default-service-account-json-key
                //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);


                bool IsImageTazOk = false;
                IsImageTazOk = true;
                //for (int i = 0; i < file.FileData.Count; i++)
                //{
                //    var source = file.FileData[i].LocalFileName;

                //    var client = ImageAnnotatorClient.Create();
                //    var image = Image.FromFile(source); //FromUri("https://ofekmanage.com/api/uploads/5397/%D7%9E%D7%99%D7%A8%D7%91%20%D7%9E%D7%98%D7%A8%D7%99%20%D7%AA.%D7%96.jpg");
                //    var labels = client.DetectText(image);

                //    string Taz = folder.Replace("taz_", "");

                //    if (string.IsNullOrEmpty(Taz) || Taz == "null") return Ok("NoTaz");



                //    var LabelContainsTaz = labels.Where(x => x.Description.Length >= 7 && Taz.Contains(x.Description)).ToList();

                //    if (LabelContainsTaz.Count > 0)
                //    {
                //        IsImageTazOk = true;
                //    }

                //}

                if (!IsImageTazOk)
                {

                    for (int i = 0; i < file.FileData.Count; i++)
                    {
                        var source = file.FileData[i].LocalFileName;
                        if (File.Exists(source))
                        {
                            File.Delete(source);
                        }

                    }



                    return Ok(false);
                };


            }

            if (folder.Contains("Companies_Logo"))
            {
                if (!Directory.Exists(WorkerPath + "/Logo/"))
                {
                    Directory.CreateDirectory(WorkerPath + "/Logo/");

                }

                root = root + "/Logo";
            }

            if (folder.Contains("Companies_Sign"))
            {
                if (!Directory.Exists(WorkerPath + "/PDFS/"))
                {
                    Directory.CreateDirectory(WorkerPath + "/PDFS/");

                }

                // var rootss = HttpContext.Current.Server.MapPath("~/App_Data/Companies/59/Logo/");

                root = root + "/PDFS";
            }

            if (folder.Contains("PDFS_ALL"))
            {
               
                if (!Directory.Exists(WorkerPath + "/PDFS/"))
                {
                    Directory.CreateDirectory(WorkerPath + "/PDFS/");

                }


                root = root + "/PDFS";
            }

            string fileList = "";
            for (int i = 0; i < file.FileData.Count; i++)
            {
                var source = file.FileData[i].LocalFileName;

                var dest = root + "/" + file.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", "");
                //dest = filterFilename(dest);

                if (folder.Contains("Companies_Sign"))
                {
                    dest = root + "/SignatureAmuta.png";

                }


               



                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }


                File.Move(source, dest);


                if (i == 0)
                {
                    fileList += Path.GetFileName(dest);

                }
                else
                {
                    fileList += "," + Path.GetFileName(dest);
                }

                if (folder.Contains("PDFS_ALL"))
                {
                    var CurrentFileName = Path.GetFileName(dest);

                    using (var Context = new Context())
                    {
                        var FarmPDFFilesList = Context.FarmPDFFiles.Where(x => x.FarmId == workerid).OrderByDescending(x => x.Seq).ToList();


                        FarmPDFFiles farmPDFFiles = new FarmPDFFiles();
                        farmPDFFiles.FarmId = workerid;
                        farmPDFFiles.StatusId = 1;
                        farmPDFFiles.FileName = CurrentFileName;
                        farmPDFFiles.Is101 = false;

                        if (FarmPDFFilesList.Count > 0) 
                            farmPDFFiles.Seq = FarmPDFFilesList[0].Seq + 1;
                        else
                            farmPDFFiles.Seq = 1;


                        bool IsExistFileName = FarmPDFFilesList.Any(x => x.FileName == CurrentFileName);

                        if (!IsExistFileName)
                        {

                            Context.FarmPDFFiles.Add(farmPDFFiles);
                            Context.SaveChanges();  
                        }


                    }
                }

            }


            return Ok(fileList);
        }


        //[Route("DeleteYear")]
        //[HttpGet]
        //public string DeleteYear()
        //{
        //    using (var Context = new Context())
        //    {

        //        var Wor = Context.Workers.Where(x => x.ShnatMas == "2021").ToList();
        //        foreach (var item in Wor)
        //        {
        //            DeleteWorkerLoop(item.Id, true);
        //        }


        //    }

        //    //UploadFromAccess uac = new UploadFromAccess();
        //    //uac.UpdateUsersLessons();
        //    return "sdsdsdsd";


        //}


        [Route("uploadformail/{folder}/{workerid}/{text}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadForMail(string folder, int workerid, string text)
        {




            if (folder == "send") return Ok("true");


            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            string root = HttpContext.Current.Server.MapPath("~/Uploads/");

            string WorkerPath = root + workerid.ToString();

            if (!Directory.Exists(WorkerPath))
            {
                Directory.CreateDirectory(WorkerPath);

            }

            root = WorkerPath;

            var provider = new MultipartFormDataStreamProvider(root);

            var file = await Request.Content.ReadAsMultipartAsync(provider);


            string fileList = "";


            try
            {
                var CurrentUser = new User();
                using (var Context = new Context())
                {

                    CurrentUser = Context.Users.Where(x => x.Id == workerid).FirstOrDefault();



                }


                if (text == "undefined") text = "";


                // var CurrentUser = UsersService.GetCurrentUser();

                string MailTo = ConfigurationSettings.AppSettings["MailTo"].ToString();


                string SmtpHost = ConfigurationSettings.AppSettings["SmtpHost"].ToString();
                string MailUser = ConfigurationSettings.AppSettings["MailUser"].ToString();
                string MailPassword = ConfigurationSettings.AppSettings["MailPassword"].ToString();



                SmtpClient client = new SmtpClient(SmtpHost, 25);
                client.Credentials = new System.Net.NetworkCredential(MailUser, MailPassword); //
                client.EnableSsl = false;

                string Body = "<html dir='rtl'><div style='text-align:right'><b>שלום רב,</b>" + "<br/>" + "מצ''ב קבצים חדשים.</div><br/>";// </html>";

                Body += text + "</html>";

                string Title = "הודעה חדשה - " + CurrentUser.FirstName + " " + CurrentUser.LastName;

                MailMessage actMSG = new MailMessage(
                                        "office@ofekmanage.com",
                                         MailTo,
                                        Title,
                                         Body);


                actMSG.IsBodyHtml = true;
                for (int i = 0; i < file.FileData.Count; i++)
                {
                    FileStream fStream = new FileStream(file.FileData[i].LocalFileName, FileMode.Open);

                    Attachment attachment = new Attachment(fStream, file.FileData[i].Headers.ContentDisposition.FileName.Replace("\"", ""), file.FileData[i].Headers.ContentType.MediaType);

                    actMSG.Attachments.Add(attachment);

                    // fStream.Close();

                }
                client.Send(actMSG);

            }
            catch (Exception ex)
            {
                return Ok("false");
            }
            finally
            {


            }
            return Ok(fileList);
        }




        public string filterFilename(string filename)
        {
            var suffix = 0;

            while (true)
            {
                suffix++;
                var NewFileName = Path.GetFileNameWithoutExtension(filename) + "_" + suffix;
                var OldFileName = Path.GetFileNameWithoutExtension(filename);
                var TempFilePath = filename.Replace(OldFileName, NewFileName);

                if (!File.Exists(TempFilePath))
                {
                    filename = TempFilePath;
                    break;
                }

            }
            return filename;
        }

        [Route("delete")]
        [HttpGet]
        public IHttpActionResult Delete(string filename)
        {
            string root = HttpContext.Current.Server.MapPath("~/Uploads/");
            File.Delete(root + filename);
            return Ok();
        }




    }
}

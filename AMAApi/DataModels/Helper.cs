using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mail;
using System.Web.Services.Discovery;

namespace FarmsApi.DataModels
{
    public static class Helper
    {


        public static bool ConvertToBool(string value)
        {

            Boolean myBool;

            if (Boolean.TryParse(value, out myBool))
            {
                return myBool;
            }

            return false;


        }


        public static int ConvertToInt(string value)
        {

            int myInt;

            if (Int32.TryParse(value, out myInt))
            {
                return myInt;
            }

            return 0;


        }

        public static string SendSMSEndPoint(string Phone, string message, string From = null)
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

            if (string.IsNullOrEmpty(From))
                val2.AddParameter("from", (object)"101Form");
            else
            {
                val2.AddParameter("from", (object)From);
            }

            val2.AddParameter("recipient", (object)Phone);

            val2.AddParameter("message", (object)message);

            val2.AddParameter("message_type", (object)"SMS");

            RestSharp.IRestResponse val3 = val.Execute((RestSharp.IRestRequest)(object)val2);

            return val3.Content;



            //InsertLog("INFO", "SendSmsTtsNew - " + message_type, Phone, "Response: " + val3.Content);

        }

        public static dynamic ResAsJson(string jsonString)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return jsonObj;


        }

        public static string GetConfigureValue(string Param)
        {
            string Value = ConfigurationSettings.AppSettings[Param].ToString();

            return Value;


        }


        public static User GetCurrentUser()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
           // var identity = (ClaimsIdentity)User.Identity;

            if (identity != null)
            {

                var CU = identity.Claims.SingleOrDefault(c => c.Type == "UserObj").Value;

                var CurrentUser = JsonConvert.DeserializeObject<User>(CU);

                return CurrentUser;

            }
            return null;

        }


        //**************************************************** Send Mails ********************************




        public static bool SendMail(string Subject, string Body, string To, string CC, string FarmId, bool IsNoOffice = false, List<string> FilesToSend = null)
        {


            string SmtpHost = ConfigurationSettings.AppSettings["SmtpHost"].ToString();
            string MailUser = ConfigurationSettings.AppSettings["MailUser"].ToString();
            string MailPassword = ConfigurationSettings.AppSettings["MailPassword"].ToString();

            Body = GetHeader() + Body;// + GetFooter();

            SmtpClient SmtpServer = new SmtpClient();
            System.Net.Mail.MailMessage actMSG = new System.Net.Mail.MailMessage();
            SmtpServer.Host = SmtpHost;
            SmtpServer.Port = 25;

            SmtpServer.EnableSsl = false;

            // SmtpServer.UseDefaultCredentials = false;

            string mail_user = MailUser;
            string mail_pass = MailPassword;

            SmtpServer.Credentials = new System.Net.NetworkCredential(mail_user, mail_pass);


            actMSG.IsBodyHtml = true;

            actMSG.Subject = Subject;

            // *********************************************** add image to body
            try
            {
                actMSG.Body = Body;

                var root = HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId + "/Logo/");



                if (Directory.Exists(root))
                {
                    string[] files = Directory.GetFiles(root);

                    if (files.Length > 0)
                    {

                        LinkedResource Img = new LinkedResource(files[0], MediaTypeNames.Image.Jpeg);
                        Img.ContentId = "MyImage";

                        string fullHtmlBody = $@"                    
                        <div style=""overflow-x: hidden;"">
                        {Body}
                        <img src=""cid:MyImage"" alt=""Logo"" style=""float:right;""  />
                         </div>                ";
                        var htmlView = AlternateView.CreateAlternateViewFromString(fullHtmlBody, null, MediaTypeNames.Text.Html);
                        htmlView.LinkedResources.Add(Img);
                        actMSG.AlternateViews.Add(htmlView);

                        actMSG.Body = fullHtmlBody;
                    }



                }


            }
            catch (Exception ex)
            {
                actMSG.Body = Body;
            }

            //********************************************
            if (FilesToSend != null)
                foreach (var file in FilesToSend)
                {
                    actMSG.Attachments.Add(new Attachment(file));
                }



            // נועד בכדי לשלוח גם לשאר האנשים
            if (!string.IsNullOrEmpty(To))
            {

                foreach (string address in To.Split(','))
                {
                    actMSG.To.Add(address.Trim());
                }

                //actMSG.To.Add(To);
            }

            // נועד בכדי לשלוח גם לשאר האנשים
            if (!string.IsNullOrEmpty(CC))
            {
                actMSG.CC.Add(CC);
            }

            //if (!string.IsNullOrEmpty(officeMails) && !IsNoOffice)
            //{
            //    actMSG.Bcc.Add(officeMails);
            //}

            actMSG.From = new MailAddress(mail_user);

            try
            {

                SmtpServer.Send(actMSG);
                actMSG.Dispose();


                return true;

            }
            catch (Exception ex)
            {
                // HttpContext.Current.Response.Write(ex.Message);
                return false;
            }

        }




        private static string GetHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<html><head></head><body dir='rtl' style='padding:5px'>");

            return sb.ToString();
        }




        private static string GetFooter()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<div>צוות ביפרוון<br /><br /></div>");

            return sb.ToString();

        }


        //**************************************************** Send Mails End ********************************

    }
}
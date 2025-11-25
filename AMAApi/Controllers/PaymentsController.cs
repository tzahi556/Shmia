using FarmsApi.DataModels;
using Google.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Google.Apis.Requests.BatchRequest;

namespace FarmsApi.Services
{
    [RoutePrefix("payments")]
    public class PaymentsController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("GetPaymentLink_PayPlus")]
        public async Task<IHttpActionResult> GetPaymentLink_PayPlus()
        {
            //[FromBody] GeneratePaymentLinkRequest generatePaymentLinkRequest
            PayPlus PayPlus = new PayPlus();


            //// הגדרת אופציות סריאליזציה גלובליות – לא שולחים שדות שהם null
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                //PropertyNamingPolicy = JsonNamingPolicy.CamelCase // אם כבר שמות ה-JSON מסומנים ב-[JsonPropertyName] אין חובה
            };

            var req = new GeneratePaymentLinkRequest
            {
                PaymentPageUid = PayPlus._PaymentPageUid,
                ChargeMethodCode = ChargeMethod.Charge,
                SendEmailApproval = true,
                SendEmailFailure = false,
                ChargeDefault = "credit-card",
                Amount = 11,
                RefUrlCallback = "https://play.svix.com/in/e_h4dREnceXUcFTMEV2Nyyf7rmQdI/",
                RefUrlSuccess = "https://paymentsdev.payplus.co.il/",
                RefUrlFailure = "https://paymentsdev.payplus.co.il/",
                RefUrlCancel = "https://paymentsdev.payplus.co.il/",

                //InitialInvoice = true,
                // object – אם אין לך ערכים, אפשר לשלוח {}:
                Customer = new CustomerDto
                {
                    //CustomerUid = "CUST-001",
                    //CustomerName = "צחיאל חזן",
                    //Email = "tzahi556@gmail.com",
                    //Phone = "0505913817"
                },

                // items – מערך של אובייקטים
                Items = new List<ItemDtoPayPlus>
                {
                    new ItemDtoPayPlus {Price = 11, Name = "מוצר לדוגמה"  } //Name = generatePaymentLinkRequest.MoreInfo
                }

            };

            req.ApplyDefaults();


            var json = System.Text.Json.JsonSerializer.Serialize(req, jsonOptions);

            var res = await PayPlus.GenericACTION("PaymentPages/generateLink", Method.POST, null, json);

            var result = System.Text.Json.JsonSerializer.Deserialize<PayPlusPaymentPageResponse>(res);

            return Ok(result);

            //return Content(res ?? "{}", "application/json");

        }





    }

  



}
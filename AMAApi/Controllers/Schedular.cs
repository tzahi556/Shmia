using FarmsApi.DataModels;
using Google.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Google.Apis.Requests.BatchRequest;

namespace FarmsApi.Services
{
    [RoutePrefix("schedular")]
    public class SchedularController : ApiController
    {
        [HttpGet]
        [Route("SchedularShmia/{param1}/{param2}")]
        public async Task<IHttpActionResult> SchedularShmia(string param1, string param2)
        {

            using (var Context = new Context())
            {
                var WorkersToRemove = Context.Workers.Where(x=>string.IsNullOrEmpty(x.FirstName) && string.IsNullOrEmpty(x.LastName) && string.IsNullOrEmpty(x.Taz) && string.IsNullOrEmpty(x.PhoneSelular)).ToList();
                Context.Workers.RemoveRange(WorkersToRemove);

                var CampainsToRemove = Context.Campains.Where(x => string.IsNullOrEmpty(x.Name)).ToList();
                Context.Campains.RemoveRange(CampainsToRemove);
                Context.SaveChanges();


            }



            return Ok(param1 + " " + param2);
        }

      




    }

  



}
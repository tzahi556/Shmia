using FarmsApi.DataModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

                var Wor = Context.Workers.Where(x => x.ShnatMas == "2021").ToList();
                foreach (var item in Wor)
                {
                    DeleteWorkerLoop(item.Id,true);
                }


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
        [Authorize]
        [Route("getFiles/{workerid}")]
        [HttpGet]
        public IHttpActionResult GetFiles(int Workerid)
        {
            return Ok(UsersService.GetFiles(Workerid));
        }

        [Authorize]
        [Route("getWorkers/{isnew}")]
        [HttpGet]
        public IHttpActionResult GetWorkers(bool isnew)
        {
            return Ok(UsersService.GetWorkers(isnew));
        }

        [Authorize]
        [Route("getWorker/{id}")]
        [HttpGet]
        public IHttpActionResult GetWorker(int id)
        {
            return Ok(UsersService.GetWorker(id));
        }

        [Authorize]
        [Route("deleteWorker/{id}/{isnew}")]
        [HttpGet]
        public IHttpActionResult DeleteWorker(int id, bool isnew)
        {

            return Ok(UsersService.DeleteWorker(id, isnew));
        }


        [Authorize]
        [Route("getWorkerChilds/{id}")]
        [HttpGet]
        public IHttpActionResult GetWorkerChilds(int id)
        {

            return Ok(UsersService.GetWorkerChilds(id));
        }


        [Authorize]
        [Route("updateWorker/{type}")]
        [HttpPost]
        public IHttpActionResult UpdateWorkerAndFiles(JArray dataobj, int type)
        {
            return Ok(UsersService.UpdateWorkerAndFiles(dataobj, type));
        }

        [Authorize]
        [Route("setUserDevice")]
        [HttpPost]
        public IHttpActionResult SetUserDevice(JObject dataobj)
        {
            //dataobj["DeviceEnter"].ToString();
            UsersService.AddEnterLog(dataobj);
            return Ok();
        }





        //******************************************** End Workers *****************************
        //******************************************** Master Table *****************************
        [Authorize]
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
    }
}

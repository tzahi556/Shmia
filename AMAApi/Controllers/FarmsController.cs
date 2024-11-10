using FarmsApi.DataModels;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http;

namespace FarmsApi.Services
{
    [RoutePrefix("farms")]
    public class FarmsController : ApiController
    {
        [Authorize]
        [Route("getFarms")]
        [HttpGet]
        public IHttpActionResult GetFarms(bool deleted = false)
        {
            return Ok(FarmsService.GetFarms(deleted));
        }

        [Authorize]
        [Route("getFarm/{id}")]
        [HttpGet]
        public IHttpActionResult GetFarm(int id)
        {
            return Ok(FarmsService.GetFarm(id));
        }

        [Authorize]
        [Route("newFarm")]
        [HttpGet]
        public IHttpActionResult NewFarm()
        {
            return Ok(new Farm());
        }

        [Authorize]
        [Route("deleteFarm/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteFarm(int id)
        {
            FarmsService.DeleteFarm(id);
            return Ok();
        }

        [Authorize(Roles = "sysAdmin")]
        [Route("updateFarm")]
        [HttpPost]
        public IHttpActionResult UpdateFarm(Farm farm)
        {
            return Ok(FarmsService.UpdateFarm(farm));
        }

        [Authorize]
        [Route("updateFarmInvoice")]
        [HttpPost]
        public IHttpActionResult UpdateFarmInvoice(Farm farm)
        {
            return Ok();
           
        }

        [Authorize]
        [Route("getMangerFarm")]
        [HttpGet]
        public IHttpActionResult GetMangerFarm()
        {
            return Ok(FarmsService.GetMangerFarm());
        }


        [Authorize]
        [Route("getMangerInstructorFarm")]
        [HttpGet]
        public IHttpActionResult GetMangerInstructorFarm()
        {
            return Ok(FarmsService.GetMangerInstructorFarm());
        }


        [Authorize]
        [Route("setMangerInstructorFarm")]
        [HttpPost]
        public IHttpActionResult SetMangerInstructorFarm(List<FarmInstructors> FarmInstructors)
        {
            return Ok(FarmsService.SetMangerInstructorFarm(FarmInstructors));
        }


        [Authorize]
        [Route("setMangerFarm")]
        [HttpPost]
        public IHttpActionResult SetMangerFarm(FarmManagers farmmanger)
        {
            return Ok(FarmsService.SetMangerFarm(farmmanger));
        }

        [Authorize]
        [Route("getFarmsMainUser/{FarmId}")]
        [HttpGet]
        public IHttpActionResult GetFarmsMainUser(int FarmId)
        {
            return Ok(FarmsService.GetFarmsMainUser(FarmId));
        }


        [Authorize]
        [Route("getKlalitHistoris")]
        [HttpGet]

        public IHttpActionResult GetKlalitHistoris(int FarmId , string startDate = null, string endDate = null,int? type=null,int? klalitId = null)
        {
            return Ok(FarmsService.GetKlalitHistoris(FarmId, startDate, endDate,type,klalitId));
        }


        [Authorize]
        [Route("setKlalitHistoris")]
        [HttpPost]

        public IHttpActionResult SetKlalitHistoris(KlalitHistoris kh)
        {
            return Ok(FarmsService.SetKlalitHistoris(kh));
        }


    }
}

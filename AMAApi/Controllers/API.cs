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
using System.Web.Http;
using static Google.Apis.Requests.BatchRequest;

namespace FarmsApi.Services
{
    [RoutePrefix("api")]
    public class APIController : ApiController
    {


        [HttpGet]
        [Route("GetToken4API/{username}/{password}")]
        public async Task<IHttpActionResult> GetToken4API(string username, string password)
        {

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var authService = new AuthService(baseUrl);
            string token = "";
            try
            {
                token = await authService.LoginAndGetTokenAsync(username, password);
                // token = await authService.LoginAndGetTokenAsync("tzahi556@gmail.com", "123");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("שם משתמש או סיסמה לא נכונים");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("שגיאה בקבלת טוקן: " + ex.Message));
            }

            return Ok(new
            {
                Status = "200",
                AccessToken = token
            });

        }


        //******************************************** Workers *****************************
        [Authorize]
        [Route("API_Workers/{type}")]
        [HttpPost]
        public IHttpActionResult API_Workers(int type, [FromBody] ApiResponse DataObj)
        {
            using (var Context = new Context())
            {

                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "dd/MM/yyyy",
                    Culture = System.Globalization.CultureInfo.InvariantCulture
                };




                string RanadKey = DataObj.client.apiKey;

                Farm farm = Context.Farms.Where(x => x.RanadKey == RanadKey).FirstOrDefault();
                if (farm == null)
                {

                    farm = new Farm();
                    farm.Id = 0;
                    farm.Name = DataObj.client.name;
                    farm.Address = DataObj.client.address;
                    farm.IdNumber = DataObj.client.taxId;

                    farm.OfficeNumber = DataObj.client.phoneNumber;
                    farm.OfficeMail = DataObj.client.emailAddress;

                    farm.ContactNumber = DataObj.client.contactPhoneNumber;
                    farm.ContactName = DataObj.client.contactName;

                    farm.Logo = DataObj.client.logoURL;

                    farm.RanadKey = RanadKey;
                    farm.StatusId = 1;

                    Context.Farms.Add(farm);
                    Context.SaveChanges();

                }

                int FarmId = farm.Id;


                var WorkersExistList = Context.Workers.Where(x => x.FarmId == FarmId).ToList();
                var DepartmentsExistList = Context.Departments.Where(x => x.FarmId == FarmId).ToList();


                var WorkersExistIds = WorkersExistList.Select(x => x.Taz).ToList();

                var Workers = DataObj.workers;

                foreach (var Worker in Workers)
                {
                    var IsNew = false;

                    var ExistsWorker = WorkersExistList.Where(x => x.RanadId == Worker.id).FirstOrDefault();

                    if (ExistsWorker == null)
                    {
                        ExistsWorker = new Workers();
                        IsNew = true;
                    }

                    ExistsWorker.FirstName = Worker.firstName;
                    ExistsWorker.LastName = Worker.lastName;
                    ExistsWorker.RanadId = Worker.id;
                    ExistsWorker.Taz = Worker.idNumber;
                    ExistsWorker.FarmId = FarmId;
                    ExistsWorker.BirthDate = Helper.ConvertToDatetime(Worker.birthDate);
                    ExistsWorker.Email = Worker.emailAddress;
                    ExistsWorker.PhoneSelular = Worker.phoneNumber;

                    ExistsWorker.City = Worker.address.city;
                    ExistsWorker.Mikud = Worker.address.zipcode;
                    ExistsWorker.Street = Worker.address.street;
                    ExistsWorker.HouseNumber = Worker.address.houseNumber;
                    ExistsWorker.StatusId = (Worker.isDisabled) ? 0 : 1;


                    if (string.IsNullOrEmpty(ExistsWorker.City) && string.IsNullOrEmpty(ExistsWorker.Mikud) && string.IsNullOrEmpty(ExistsWorker.Street) && string.IsNullOrEmpty(ExistsWorker.HouseNumber))
                    {
                        ExistsWorker.Street = Worker.address.full;
                    }

                    // מחלקות ואגפים
                    int EntityId = GetEntityId(FarmId, 0, Worker, DepartmentsExistList);
                    if (EntityId != 0) ExistsWorker.FactoryId = EntityId;

                    EntityId = GetEntityId(FarmId, 1, Worker, DepartmentsExistList);
                    if (EntityId != 0) ExistsWorker.DivisionsId = EntityId;

                    EntityId = GetEntityId(FarmId, 2, Worker, DepartmentsExistList);
                    if (EntityId != 0) ExistsWorker.SubDivisionsId = EntityId;

                    EntityId = GetEntityId(FarmId, 3, Worker, DepartmentsExistList);
                    if (EntityId != 0) ExistsWorker.DepartmentsId = EntityId;

                    EntityId = GetEntityId(FarmId, 4, Worker, DepartmentsExistList);
                    if (EntityId != 0) ExistsWorker.SubDepartmentsId = EntityId;


                    Context.Workers.AddOrUpdate(ExistsWorker);

                    // משתמשים והרשאות
                    var RolesIds = Context.Roles.Where(x => x.Id > 1).Select(x => x.Id).ToList();

                    var CurrentRolesId = Worker.user.role.id;

                    if (RolesIds.Any(x => x == CurrentRolesId))
                    {

                        var ExistsUser = Context.Users.Where(x => x.RanadId == Worker.id).FirstOrDefault();

                        if (ExistsUser == null)
                        {
                            ExistsUser = new User();
                        }

                       
                        ExistsUser.FirstName = Worker.firstName;
                        ExistsUser.LastName = Worker.lastName;
                        ExistsUser.RolesId = CurrentRolesId;

                        ExistsUser.Email = Worker.emailAddress;

                        Context.Users.AddOrUpdate(ExistsUser);
                        Context.SaveChanges();

                        Context.Database.ExecuteSqlCommand(
                          "EXEC dbo.[SetUser] @UserId, @Password",
                          new SqlParameter("@UserId", ExistsUser.Id),
                          new SqlParameter("@Password", Worker.user.password)
                         );

                        // מנהל מחלקות
                        if (CurrentRolesId == 6)
                        {
                           // List<int> DepartmentsIds = new List<int>();

                           // DepartmentsIds = DepartmentsExistList.Select(x => x.Id).Any().ToList();


                           // DepartmentsIds.Add((int)ExistsWorker.DepartmentsId);

                            List<int> DepartmentsRanadIds = DepartmentsExistList.Select(x => x.RanadId ?? 0).ToList();


                            var set = new HashSet<int>(DepartmentsRanadIds);

                            List<int> commonIds = DepartmentsRanadIds
                                                                    .Where(id => set.Contains(id))
                                                                    .ToList();

                            //   DepartmentsIds.AddRange(Worker.user.role.filters.departmentIds);



                        }


                    }


                    //שמירה סופית
                    Context.SaveChanges();
                }
            }



            return Ok(new
            {
                Status = 200,
                Success = true,
                Message = "העדכון בוצע בהצלחה"
            });

        }

        private int GetEntityId(int FarmId, int TypeId, Worker Worker, List<Departments> DepartmentsExistList)
        {
            using (var Context = new Context())
            {
                int Res = 0;

                if (TypeId == 0 && Worker.factory.id > 0)
                {

                    var ExistsFactory = DepartmentsExistList.Where(x => x.RanadId == Worker.factory.id && x.TypeId == TypeId).FirstOrDefault();

                    if (ExistsFactory == null)
                    {
                        ExistsFactory = new Departments();
                    }


                    ExistsFactory.FarmId = FarmId;
                    ExistsFactory.TypeId = 0;
                    ExistsFactory.Name = Worker.factory.name;
                    ExistsFactory.StatusId = 1;
                    ExistsFactory.RanadId = Worker.factory.id;

                    Context.Departments.AddOrUpdate(ExistsFactory);
                    Context.SaveChanges();

                    Res = ExistsFactory.Id;


                }

                if (TypeId == 1 && Worker.division.id > 0)
                {

                    var ExistsEntity = DepartmentsExistList.Where(x => x.RanadId == Worker.division.id && x.TypeId == TypeId).FirstOrDefault();

                    if (ExistsEntity == null)
                    {
                        ExistsEntity = new Departments();
                    }


                    ExistsEntity.FarmId = FarmId;
                    ExistsEntity.TypeId = TypeId;
                    ExistsEntity.Name = Worker.division.name;
                    ExistsEntity.StatusId = 1;
                    ExistsEntity.RanadId = Worker.division.id;

                    Context.Departments.AddOrUpdate(ExistsEntity);
                    Context.SaveChanges();

                    Res = ExistsEntity.Id;


                }

                if (TypeId == 2 && Worker.subDivision.id > 0)
                {

                    var ExistsEntity = DepartmentsExistList.Where(x => x.RanadId == Worker.subDivision.id && x.TypeId == TypeId).FirstOrDefault();

                    if (ExistsEntity == null)
                    {
                        ExistsEntity = new Departments();
                    }


                    ExistsEntity.FarmId = FarmId;
                    ExistsEntity.TypeId = TypeId;
                    ExistsEntity.Name = Worker.subDivision.name;
                    ExistsEntity.StatusId = 1;
                    ExistsEntity.RanadId = Worker.subDivision.id;

                    Context.Departments.AddOrUpdate(ExistsEntity);
                    Context.SaveChanges();

                    Res = ExistsEntity.Id;


                }

                if (TypeId == 3 && Worker.department.id > 0)
                {

                    var ExistsEntity = DepartmentsExistList.Where(x => x.RanadId == Worker.department.id && x.TypeId == TypeId).FirstOrDefault();

                    if (ExistsEntity == null)
                    {
                        ExistsEntity = new Departments();
                    }


                    ExistsEntity.FarmId = FarmId;
                    ExistsEntity.TypeId = TypeId;
                    ExistsEntity.Name = Worker.department.name;
                    ExistsEntity.StatusId = 1;
                    ExistsEntity.RanadId = Worker.department.id;

                    Context.Departments.AddOrUpdate(ExistsEntity);
                    Context.SaveChanges();

                    Res = ExistsEntity.Id;


                }

                if (TypeId == 4 && Worker.subDepartment.id > 0)
                {

                    var ExistsEntity = DepartmentsExistList.Where(x => x.RanadId == Worker.subDepartment.id && x.TypeId == TypeId).FirstOrDefault();

                    if (ExistsEntity == null)
                    {
                        ExistsEntity = new Departments();
                    }


                    ExistsEntity.FarmId = FarmId;
                    ExistsEntity.TypeId = TypeId;
                    ExistsEntity.Name = Worker.subDepartment.name;
                    ExistsEntity.StatusId = 1;
                    ExistsEntity.RanadId = Worker.subDepartment.id;

                    Context.Departments.AddOrUpdate(ExistsEntity);
                    Context.SaveChanges();

                    Res = ExistsEntity.Id;


                }

                return Res;
            }


        }



        //******************************************** Workers *****************************
        [HttpGet]
        [Route("TestWorkersPost")]
        public async Task<IHttpActionResult> GetTestWorkersPost()
        {


            var authHeader = Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                return BadRequest("חסר טוקן מסוג Bearer");
            }

            string token = authHeader.Parameter;


            string apiUrl = "http://localhost:44033/api/API_Workers/1"; // שנה לכתובת הנכונה

            // טוקן במידת הצורך
            // string token = "eyJhbGciOi...";

            JArray workers = new JArray
    {
        new JObject
        {
            { "Name", "משה כהן" },
            { "Phone", "0521234567" }
        },
        new JObject
        {
            { "Name", "רות לוי" },
            { "Phone", "0539876543" }
        }
    };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                StringContent content = new StringContent(workers.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string result = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    Status = response.StatusCode,
                    Response = result
                });
            }
        }

        //****************************************************************************

        [HttpGet]
        [Route("TestGetRanadWorker")]
        public async Task<IHttpActionResult> TestGetRanadWorker()
        {

            //// var service = new WorkerApiService();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Culture = System.Globalization.CultureInfo.InvariantCulture
            };

            //string json = File.ReadAllText("C:\\Users\\Tzahi\\OneDrive\\Desktop\\Shiran\\response_1752604359193.json"); // או JSON ישירות ממחרוזת
            //List<Worker> worker = JsonConvert.DeserializeObject<List<Worker>>(json, settings);



            var json = await WorkerApiService.GetWorkersAsync("V4NUTC8N4KF6PRA12ATNXV", new List<int>(), new List<int> { 0, 1, 2, 11, 4, 5, 100, 101 }, false); //new List<int> { 1,2,3,4,5,6 }
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(json, settings);
            // List<Worker> workers = JsonConvert.DeserializeObject<List<Worker>>(json, settings);

            var distinctFactories = result.workers
                              .Select(w => w.factory.id)

                              .Distinct()
                              .ToList();




            return Ok(result);

        }




        [Route("API_SetWorkers/{type}")]
        [HttpPost]
        public IHttpActionResult API_SetWorkers(List<Worker> WorkerList, int type)
        {


            return Ok();

        }




    }

    public class AuthService
    {
        private readonly string _baseUrl;

        public AuthService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<string> LoginAndGetTokenAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var tokenEndpoint = _baseUrl.TrimEnd('/') + "/token";

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password)
            });

                var response = await client.PostAsync(tokenEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"שגיאה בקבלת טוקן: {response.StatusCode}\n{responseString}");

                var tokenObj = JObject.Parse(responseString);
                return tokenObj["access_token"]?.ToString();
            }
        }
    }

    public class GetWorkersExtendedRequest
    {
        public string ApiKey { get; set; }
        public List<int> workerIds { get; set; } = new List<int>();
        public List<int> factoryIds { get; set; } = new List<int>();

        public string forwardToURL { get; set; }

        public bool IncludeDisabled { get; set; }
    }

    public static class WorkerApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string _url = "https://api.isufitcore.com/Workers/GetWorkersExtended";

        public static async Task<string> GetWorkersAsync(string apiKey, List<int> workerIds, List<int> factoryIds, bool includeDisabled = true)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var request = new GetWorkersExtendedRequest
            {
                ApiKey = apiKey,
                workerIds = workerIds,
                factoryIds = factoryIds,
                IncludeDisabled = includeDisabled,
                forwardToURL = "https://api.isufitcore.com/"

            };

            var response = await _httpClient.PostAsJsonAsync(_url, request);

            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return json; // מחזיר את ה-JSON הגולמי
            }
            else
            {
                // מחזיר שגיאה בתוך JSON
                return $"{{\"error\": \"{response.StatusCode}\", \"details\": {System.Text.Json.JsonSerializer.Serialize(json)} }}";
            }
        }
    }

    /// <summary>
    /// api ranad
    /// </summary>
    public class ApiResponse
    {
        public Client client { get; set; }
        public List<Worker> workers { get; set; }
        public bool success { get; set; }
        public bool isAuthenticated { get; set; }
        public string messageEN { get; set; }
        public string messageHE { get; set; }
    }

    public class Client
    {
        public string id { get; set; }
        public string taxId { get; set; }
        public string apiKey { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public string contactPhoneNumber { get; set; }
        public string contactName { get; set; }
        public string emailAddress { get; set; }
        public string logoURL { get; set; }


    }

    public class Worker
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string otherName { get; set; }
        public string fatherName { get; set; }
        public string idNumber { get; set; }



        public string passportNumber { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string isufitStartDate { get; set; }
        public int jobPercentage { get; set; }
        public string phoneNumber { get; set; }
        public string mobileNumber { get; set; }
        public string emailAddress { get; set; }
        public int minimumBreak { get; set; }
        public UserApi user { get; set; }
        public Address address { get; set; }
        public Factory factory { get; set; }
        public Division division { get; set; }
        public SubDivision subDivision { get; set; }
        public Department department { get; set; }
        public SubDepartment subDepartment { get; set; }
        public JobPosition jobPosition { get; set; }
        public Contract contract { get; set; }
        public DirectManager directManager { get; set; }
        public HealthInsitute healthInsitute { get; set; }
        public MaritalStatus maritalStatus { get; set; }
        public List<JobPercentageByDate> jobPercentageByDate { get; set; }
        public bool isDisabled { get; set; }
    }

    public class Address
    {
        public string full { get; set; }
        public string street { get; set; }
        public string houseNumber { get; set; }
        public string apartmentNumber { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zipcode { get; set; }
    }

    public class Factory
    {
        public string taxId { get; set; }
        public string address { get; set; }
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class Division
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class SubDivision
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class Department
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class SubDepartment
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class JobPosition
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class Contract
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class DirectManager
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class HealthInsitute
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class MaritalStatus
    {
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }


    public class JobPercentageByDate
    {
        public int WorkerNumber { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Percentage { get; set; }
    }


    public class Filters
    {
        public List<int> factoryIds { get; set; }
        public List<int> divisionIds { get; set; }
        public List<int> departmentIds { get; set; }
        public List<int> jobPositionIds { get; set; }
        public List<int> subDivisionIds { get; set; }
        public List<int> subDepartmentIds { get; set; }
        public List<int> workerIds { get; set; }
    }

    public class Role
    {
        public Filters filters { get; set; }
        public int id { get; set; }
        public int permanentId { get; set; }
        public string name { get; set; }
    }

    public class UserApi
    {
        public string username { get; set; }
        public string password { get; set; }
        public Role role { get; set; }
    }



}
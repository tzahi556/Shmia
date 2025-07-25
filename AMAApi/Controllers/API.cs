using FarmsApi.DataModels;
using Google.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
                if(farm == null)
                {

                    farm = new Farm();
                    farm.Id = 0;
                    farm.Name = DataObj.client.name;
                    farm.StatusId = 1;

                    Context.Farms.Add(farm);
                    Context.SaveChanges();

                }

                int FarmId = farm.Id;


                var WorkersExistList = Context.Workers.Where(x=>x.FarmId== FarmId).ToList();

                var WorkersExistIds = WorkersExistList.Select(x => x.Taz).ToList();

                var Workers = DataObj.workers;

                foreach (var Worker in Workers)
                {
                    //כבר קיים עובד כזה
                    if (WorkersExistIds.Contains(Worker.idNumber))
                    {

                    }
                }
            }



            return Ok(new
            {
                Status = 200,
                Success = true,
                Message = "העדכון בוצע בהצלחה"
            });

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
        public string apiKey { get; set; }
        public string name { get; set; }
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
        public int taxId { get; set; }
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
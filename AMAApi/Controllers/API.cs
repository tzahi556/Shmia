using FarmsApi.DataModels;
using Google.Rpc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public IHttpActionResult API_Workers(JArray dataobj, int type)
        {

            return Ok("אזשי");
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

           // var service = new WorkerApiService();
           var res = await WorkerApiService.GetWorkersAsync("00EE8FDD54EAB3A37C54DB", new List<int>(), true);


            return Ok(res);

        }


    }

//    using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;

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

}

public class GetWorkersExtendedRequest
{
    public string ApiKey { get; set; }
    public List<int> Ids { get; set; } = new List<int>();
    public bool IncludeDisabled { get; set; }
}


public static class WorkerApiService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string _url = "https://api.isufitcore.com/Workers/GetWorkersExtended";

    public static async Task<string> GetWorkersAsync(string apiKey, List<int> ids, bool includeDisabled = true)
    {

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        var request = new GetWorkersExtendedRequest
        {
            ApiKey = apiKey,
            Ids = ids,
            IncludeDisabled = includeDisabled
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
            return $"{{\"error\": \"{response.StatusCode}\", \"details\": {JsonSerializer.Serialize(json)} }}";
        }
    }
}

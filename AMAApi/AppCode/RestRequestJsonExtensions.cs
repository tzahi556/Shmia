using Newtonsoft.Json;
using RestSharp;

namespace FarmsApi.Services   // או כל namespace אחר שמתאים לך
{
    public static class RestRequestJsonExtensions
    {
        /// <summary>
        /// התאמה של AddJsonBody לגרסת RestSharp הישנה (.NET 4)
        /// </summary>
        public static void AddJsonBody(this RestRequest request, object body)
        {
            if (body == null)
                return;

            string json;

            // אם כבר הגיע string – נניח שזה JSON מוכן ולא נסריאל מחדש
            if (body is string s)
            {
                json = s;
            }
            else
            {
                json = JsonConvert.SerializeObject(body);
            }

            // כמו AddJsonBody בגרסאות החדשות
            request.AddParameter("application/json", json, ParameterType.RequestBody);
        }
    }
}

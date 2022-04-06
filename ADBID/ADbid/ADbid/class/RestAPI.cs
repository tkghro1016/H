using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADbid
{

    // 1. JSON.NET 설치 (NuGet)
    //     PM> install-package Newtonsoft.Json
    //
    // 2. Newtonsoft.Json.dll 참조

    public class RestAPI
    {
        private string uri;

        public RestAPI()
        {
            uri = "http://adbid.co.kr";

        }

        public string[] LogIn(string ID, string PW)
        {
            try
            {
                var cli = new WebClient();

                cli.QueryString.Add("user_id", ID);
                cli.QueryString.Add("user_pw", PW);

                var response = cli.UploadValues(uri + "/member/api/login", "POST", cli.QueryString);
                var responseString = UnicodeEncoding.UTF8.GetString(response);
                JObject jObject = JObject.Parse(responseString);

                string[] returnString = { JsonConvert.SerializeObject(jObject["resultMessage"]), JsonConvert.SerializeObject(jObject["resultCode"]) };

                return new string[] { returnString[0].Replace("\"", ""), returnString[1].Replace("\"", "") };
            }
            catch
            {
                return new string[] { null, null };
            }

        }



        public string[] LogOut(string ID)
        {
            try
            {
                var cli = new WebClient();

                cli.QueryString.Add("user_id", ID);

                var response = cli.UploadValues(uri + "/member/api/logout", "POST", cli.QueryString);
                var responseString = UnicodeEncoding.UTF8.GetString(response);
                JObject jObject = JObject.Parse(responseString);

                string[] returnString = { JsonConvert.SerializeObject(jObject["resultMessage"]), JsonConvert.SerializeObject(jObject["resultCode"]) };

                return new string[] { returnString[0].Replace("\"", ""), returnString[0].Replace("\"", "") };
            }
            catch
            {
                return new string[] { null, null };
            }
        }


        public string Period(string ID)
        {
            try
            {
                var cli = new WebClient();

                cli.Headers[HttpRequestHeader.ContentType] = "application/json";

                string response = cli.DownloadString(uri + "/service/getEndDate?userId=" + ID);

                return response.Replace("\"", "");
            }
            catch
            {
                return null;
            }
        }

        public string[] WebLog(string ID)
        {
            try
            {
                var cli = new WebClient();

                cli.QueryString.Add("user_id", ID);

                var response = cli.UploadValues(uri + "/member/api/access", "POST", cli.QueryString);
                var responseString = UnicodeEncoding.UTF8.GetString(response);
                JObject jObject = JObject.Parse(responseString);

                string[] returnString = { JsonConvert.SerializeObject(jObject["resultMessage"]), JsonConvert.SerializeObject(jObject["resultCode"]) };

                return new string[] { returnString[0].Replace("\"", ""), returnString[0].Replace("\"", "") };
            }
            catch
            {
                return new string[] { null, null };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Valispace.Common;
using Valispace.DataAccess.Domain;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Valispace.DataAccess.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthenticationResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        public string UserName { get; set; }
        public string Domain { get; set; }
        public string Key { get; set; }

        public DateTime Created { get; set; }
        public bool IsExpired
        {
            get
            {
                return Created.AddSeconds(expires_in) <= DateTime.Now;
            }
        }
    }
}


namespace Valispace.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class APIUrlConstant
    {
        public const string AUTH = "/o/token/";
        public const string WORKSPACE = "/rest/workspace/";
        public const string PROJECT = "/rest/project/";
        public const string COMPONENTS = "/rest/components/";
        public const string COMPONENT = "/rest/components/";
        public const string CALCULATIONS = "/rest/calculations";
        public const string DATASET = "/rest/datasets/";
        public const string VALICONTAINER = "/rest/valicontainer";
        public const string VALI = "/rest/valis/";
        public const string VALI1 = "/rest/vali/";
        public const string VALINAMES = "/rest/valinames";
        public const string VALINAMES_WITH_TYPE = "/rest/valinames-with-type";
        public const string VALIMARGIN = "/rest/valimargin";
        public const string VALIREQUIREMENTS = "rest/valirequirements";
        public const string VALITYPE = "rest/valitype";
        public const string VALIHISTORY = "rest/valihistory";
        public const string CHANGE = "/rest/change";
        public const string USER = "/rest/user";
        public const string PROFILE = "/rest/profile";
        public const string TEXTVALI = "/rest/textvali";
        public const string MATRIX = "/rest/matrix";
        public const string MODELIST = "/rest/modelist";
        public const string TAG = "/rest/tag";
        public const string COMPARISON_MATRIX = "/rest/comparison/matrix/";
        public const string COMPARISON_VALI = "/rest/comparison/vali";
        public const string PROJECT_PERMISSION = "/rest/project-permission";
        public const string COMPONENT_PERMISSION = "/rest/component-permission";
        public const string WARNING = "/rest/warning";
        public const string ANALYSIS = "/rest/analysis/";
        public const string BUDGET = "/budget_json/";
    }
}


namespace ValispacePlugin
{
    static class Helper
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }

        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, HttpContent iContent=null)
        {

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.GetAsync(requestUri).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }
            
            return response;
        }


    }


    public class ValispaceAPI
    {
        public enum RequestType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH,
        };

        const string CONTENT_TYPE_JSON = "application/json";
        const string CONTENT_TYPE_FORM_URLENCODED = "application/x-www-form-urlencoded";
        const string AUTH_TOKEN_KEY = "Authorization";

        const string GRANT_TYPE_KEY = "grant_type";
        const string CLIENT_ID_KEY = "client_id";
        const string USERNAME_KEY = "username";
        const string PASSWORD_KEY = "password";

        const string GRANT_TYPE = "password";
        const string CLIENT_ID = "ValispaceREST";

        static string domainValue = string.Empty;

        public static AuthenticationResult AuthenticationResults { get; private set; } = null;

        public static string baseUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(domainValue) && !domainValue.StartsWith("http"))
                    return "https://" + domainValue;

                return domainValue;
            }
        }

        public static ValispaceAPI Connect(string domain, string username, string password)
        {
            try
            {
                var valispaceApi = new ValispaceAPI(domain, username, password);
                return valispaceApi;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private ValispaceAPI(string domain, string username, string password)
        {
            domainValue = domain;

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE_FORM_URLENCODED));

            var formData = new Dictionary<string, string>
            {
                { GRANT_TYPE_KEY, GRANT_TYPE },
                { CLIENT_ID_KEY, CLIENT_ID },
                { USERNAME_KEY, username },
                { PASSWORD_KEY, password }
            };

            Task<HttpResponseMessage> result = client.PostAsync(APIUrlConstant.AUTH, new FormUrlEncodedContent(formData));
            HttpResponseMessage response = result.Result;
            if (result.Result.IsSuccessStatusCode)
            {
                
                AuthenticationResults = JsonConvert.DeserializeObject<AuthenticationResult>((response.Content.ReadAsStringAsync()).Result);
                //JsonConvert.DeserializeObject<AuthenticationResult>(response.Content.ReadAsStringAsync().ToString()).Result;
                AuthenticationResults.Created = DateTime.Now;
            }
        }
        // For single JSON
        public static Dictionary<string,object> GetResponse(string apiPath, RequestType requestType = RequestType.GET, HttpContent content = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (AuthenticationResults == null || AuthenticationResults.IsExpired)
            {
                // LoggingManager.Log.Error("User not authenticated or session expired.");
                return data;
            }

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE_JSON));

            client.DefaultRequestHeaders.Add(AUTH_TOKEN_KEY, AuthenticationResults.token_type + " " + AuthenticationResults.access_token);
            Task<HttpResponseMessage> result = null;

            switch (requestType)
            {
                //case RequestType.DELETE:
                //    result = client.DeleteAsync(apiPath);
                //    break;

                //case RequestType.POST:
                //    result = client.PostAsync(apiPath, content);

                //    break;

                //case RequestType.PUT:
                //    result = client.PutAsync(apiPath, content);
                //    break;

                //case RequestType.PATCH:
                //    result = Helper.PatchAsync(client, apiPath, content);
                //    break;

                case RequestType.GET:
                    result = client.GetAsync(apiPath);

                    break;
            }

            if (result.Result != null)
            {
                HttpResponseMessage response = result.Result;

                //LoggingManager.Log.Info(result.Result.ToString());

                if (result.Result.IsSuccessStatusCode)
                {
                     var L1 = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<Dictionary<string, object>>(L1);
                }
                else
                {
                    //LoggingManager.Log.Error("Failed to get response for : " + apiPath + "\n " + response.ToString());
                }
            }

            //}
            //catch (Exception ex)
            //{
            //    // LoggingManager.Log.Fatal(ex.Message, ex);
            //    throw ex;
            //}
            return data;
        }
        // For JSON lists
        public static List<T> GetAPIResponse<T>(string apiPath, RequestType requestType = RequestType.GET, HttpContent content = null)
        {
            List<T> data = new List<T>();

            //try
            //{

                if (AuthenticationResults == null || AuthenticationResults.IsExpired)
                {
                    // LoggingManager.Log.Error("User not authenticated or session expired.");
                    return data;
                }

                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE_JSON));

                client.DefaultRequestHeaders.Add(AUTH_TOKEN_KEY, AuthenticationResults.token_type + " " + AuthenticationResults.access_token);
                Task<HttpResponseMessage> result = null;

                switch (requestType)
                {
                    case RequestType.DELETE:
                        result = client.DeleteAsync(apiPath);
                        break;

                    case RequestType.POST:
                        result = client.PostAsync(apiPath, content);
                        
                        break;

                    case RequestType.PUT:
                        result = client.PutAsync(apiPath, content);
                        break;

                    case RequestType.PATCH:
                        result = Helper.PatchAsync(client, apiPath, content);
                        break;

                    case RequestType.GET:
                        result = client.GetAsync(apiPath);
                        
                        break;
                }

                if (result.Result != null)
                {
                    HttpResponseMessage response = result.Result;

                    //LoggingManager.Log.Info(result.Result.ToString());

                    if (result.Result.IsSuccessStatusCode)
                    {
                    if (requestType != RequestType.GET)
                    {
                        //data = JsonConvert.DeserializeObject<List<T>>((response.Content.ReadAsStringAsync()).Result);
                        var dataobj = JsonConvert.DeserializeObject<T>((response.Content.ReadAsStringAsync()).Result); ;
                        //var L = new List<T>();
                        data.Add(dataobj);
                        //return L;
                    }
                    else
                        try
                        {
                            data = JsonConvert.DeserializeObject<List<T>>((response.Content.ReadAsStringAsync()).Result);
                        }
                        catch
                        {
                            data = new List<T>();
                            var L1 = response.Content.ReadAsStringAsync().Result;
                            

                        }
                    }
                    else
                    {
                        //LoggingManager.Log.Error("Failed to get response for : " + apiPath + "\n " + response.ToString());
                    }
                }
                
            //}
            //catch (Exception ex)
            //{
            //    // LoggingManager.Log.Fatal(ex.Message, ex);
            //    throw ex;
            //}
            return data;
        }

        public List<Dictionary<string, object>> getProjects()
        {
            return GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.PROJECT);
        }
        public List<Dictionary<string, object>> getValis(Int64 id)
        {
            return GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.VALI + "?_project=" + id);
        }

        public List<Dictionary<string, object>> getComponents(Int64 id, string name= "")
        {
            return GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.COMPONENTS + "?project__name=" + name);
            //return GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.COMPONENTS + "?project=" + id);
        }

        public Dictionary<string, object> getComponent(Int64 id)
        {
            return GetResponse(APIUrlConstant.COMPONENTS + id+'/');
        }

        public Dictionary<string, object> getVali(Int64 id)
        {
            return GetResponse(APIUrlConstant.VALI + id+'/');
        }

        public void updateVali( Int64 vali_id,string ValiName = null, string ValiUnit = null, string ValiFormula = null)
        {
            
            var data = new Dictionary<string, object>();

            if (ValiName != null)
            {
                data["shortname"] = ValiName;
            }

            if (ValiUnit != null)
            {
                data["unit"] = ValiUnit;
            }

            if (ValiFormula != null)
            {
                data["formula"] = ValiFormula;
            }

            var stringPayload = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var response = GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.VALI + vali_id + "/", RequestType.PATCH, httpContent);
        }

        public void createVali(Int64 parent_id, string ValiName, string ValiFormula, string ValiUnit = null)
        {
            var data = new Dictionary<string, object>();


            data["shortname"] = ValiName;
            data["parent"] = parent_id.ToString();

            if (ValiUnit != null)
            {
                data["unit"] = ValiUnit;
            }

            data["formula"] = ValiFormula;
            

            var stringPayload = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var response = GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.VALI, RequestType.POST, httpContent);
        }

        public void update_Dataset(Int64 dataset_id, Array Time, Array dataset, string ValiUnit=null)
        {

            //string datapts = "\"Velocity\",\"X\"\n";
            //for (int i = 0; i < dataset.Length; i++)
            //{
            //    datapts = datapts + i.ToString()+","+ dataset.GetValue(i).ToString()+"\n";

            //}

            //var requestContent = new MultipartFormDataContent();
            //var csvbytes = Encoding.ASCII.GetBytes(datapts);
            //var datacontent = new ByteArrayContent(csvbytes);
            //datacontent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/csv");
            //requestContent.Add(datacontent, "userfile", "dataset.csv");

            // FROM HERE
            //var valiFetch = getVali(dataset_id)["function_data"].ToString();
            //var JSON_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(valiFetch);
            //var datapoints = JsonConvert.DeserializeObject(JSON_data["data"].ToString());
            ////ValiUnit = "km/s";
            //var x_head = "x";
            //var y_head = "f";

            ////Dataset
            //Array datapts = new Array[dataset.Length+1,2];
            //object[,] columns = new object[dataset.Length+1,2];
            //object x = x_head;
            //object y = y_head;
            //columns[0, 0] = x_head;
            //columns[0, 1] = y_head;
            //for (int i = 0; i < dataset.Length; i++)
            //{
            //    columns[i + 1, 1] = i; // dataset.GetValue(i);
            //    columns[i + 1, 0] = i+1;
            //}


            //datapts = columns;
            //JSON_data["data"] = (object)datapts;
            //Dictionary<string, object> data = new Dictionary<string, object> ();
            //data.Add("function_data", JSON_data);
            //var stringPayload = JsonConvert.SerializeObject(data);
            //var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            //var response = GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.VALI + dataset_id+"/", RequestType.PATCH,httpContent);

            //Dataset
            Dictionary<string, object> postData = new Dictionary<string, object>();
            object[,] columns = new object[dataset.Length, 2];

            for (int i = 0; i < dataset.Length; i++)
            {
                columns[i, 1] = dataset.GetValue(i); // dataset.GetValue(i);
                columns[i, 0] = Time.GetValue(i);
            }
            postData["data"] = columns;
            var stringPayload = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var response = GetAPIResponse<Dictionary<string, object>>(APIUrlConstant.VALI + dataset_id + "/import-dataset/", RequestType.POST, httpContent);
        }

    public AuthenticationResult GetAuthenticationResult()
        {
            return AuthenticationResults;
        }
    }

    public struct Vali
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string unit { get; set; }
    }

    public struct Component
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public List<object> Items { get; set; }
    }

    public struct DataSet
    {
        public Int64 dataset_ID { get; set; }
        public Int64 vali_ID { get; set; }
        public Component parent { get; set; }
        public string Name { get; set; }
    }
}

using ClientApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ClientApp.Utils
{
    public class Calculate
    {
        private static string baseUrl = "http://localhost:5000/api/";

        public static async Task<List<CategoryView>> GetAllCategory()
        {
            HttpResponseMessage categoryResponse = await Calculate.callGetApi("Category/GetAll");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string results = categoryResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<CategoryView>>(results);
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return null;
            }
        }
        public static async Task<HttpResponseMessage> callGetApi(string url )
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.GetAsync(url);
                return response;
            }
        }
        public static async Task<HttpResponseMessage> callGetApi(string url,string token)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync(url);
                return response;
            }
        }
        public static async Task<HttpResponseMessage> callPostApi(string url, object body)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.PostAsJsonAsync(url, body);
                return response;
            }
        }
        public static async Task<HttpResponseMessage> callPostApi(string url,string token, object body)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.PostAsJsonAsync(url,body);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> callPutApi(string url, object body)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.PutAsJsonAsync(url, body);
                return response;
            }
        }
        public static async Task<HttpResponseMessage> callPutApi(string url, string token, object body)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.PutAsJsonAsync(url, body);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> callDeleteApi(string url, string token)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.DeleteAsync(url);
                return response;
            }
        }
    }
}

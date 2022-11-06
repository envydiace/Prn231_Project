using ClientApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ClientApp.Utils
{
    public class Calculate
    {
        public static string baseUrl = "http://localhost:5000/api/";

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
        public static async Task<HttpResponseMessage> callApi(string url,string token)
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
    }
}

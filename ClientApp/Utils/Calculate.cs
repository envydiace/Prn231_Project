﻿using ClientApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ClientApp.Utils
{
    public class Calculate
    {
        private static string baseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["baseUrl"];   

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

        public static async Task<ClaimView> GetAccountClaims(string accessToken)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.GetAsync("getAccountClaims?token="+ accessToken);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    ClaimView claim = JsonConvert.DeserializeObject<ClaimView>(result);
                    return claim;
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<TokenView> GetRefreshToken(int accountId, string refreshToken)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                RefreshTokenRequest request = new RefreshTokenRequest { AccountId = accountId, RefreshToken = refreshToken };
                HttpResponseMessage response = await Client.PostAsJsonAsync("RefreshToken", request);
                if( response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TokenView>(result);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

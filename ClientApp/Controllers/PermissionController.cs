using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ClientApp.Controllers
{
    
    public class PermissionController : Controller
    {
        public IActionResult Login()
        {
            HttpContext.Session.Remove(Constants._isAdmin);
            HttpContext.Session.Remove(Constants._token);
            HttpContext.Session.Remove(Constants._cusName);
            return View("~/Views/Login.cshtml");
        }

        public async Task<ActionResult> LoginAccount(LoginView loginView)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Login.cshtml");
            }
            else
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("http://localhost:5000/api/");
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await Client.PostAsJsonAsync("Login", loginView);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        HttpContext.Session.SetString(Constants._token, result.Substring(1,result.Length-2));
                        return RedirectToAction("Product", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
            }
        }
        public IActionResult Register()
        {
            return View("~/Views/Register.cshtml");
        }

        public IActionResult ForgotPass()
        {
            return View("~/Views/ForgotPass.cshtml");
        }

        public async Task< IActionResult> ResetPassword(EmailView view)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri("http://localhost:5000/api/");
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.PostAsJsonAsync("ForgotPassword?email="+ view.Email, "");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
        }

    }
}

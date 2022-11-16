using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ClientApp.Controllers
{

    public class PermissionController : Controller
    {
        public IActionResult Login(string error)
        {
            HttpContext.Session.Remove(Constants._isAdmin);
            HttpContext.Session.Remove(Constants._accessToken);
            HttpContext.Session.Remove(Constants._cusName);
            ViewData["error"] = error;
            return View("~/Views/Login.cshtml");
        }
        public async Task<IActionResult> Logout(string token)
        {
            using (var Client = new HttpClient())
            {
                HttpResponseMessage response = await Calculate.callPostApi("Logout", token, null);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }
        }

        public async Task<ActionResult> LoginAccount(LoginView loginView)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Login.cshtml");
            }
            else
            {
                HttpResponseMessage response = await Calculate.callPostApi("Login", loginView);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    TokenView token = JsonConvert.DeserializeObject<TokenView>(result);
                    HttpContext.Session.SetString(Constants._accessToken, token.AccessToken);
                    HttpContext.Session.SetString(Constants._refreshToken, token.RefreshToken);
                    ClaimView claim = await Calculate.GetAccountClaims(token.AccessToken);
                    if (claim.Role == 1)
                    {
                        HttpContext.Session.SetString(Constants._isAdmin, "true");
                        return RedirectToAction("Product", "Admin");
                    }
                    else
                    {
                        HttpContext.Session.SetString(Constants._isAdmin, "false");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Permission", new { error = "Email or Password is not correct!" });
                }

            }
        }
        public IActionResult Register()
        {
            return View("~/Views/Register.cshtml");
        }

        public async Task<ActionResult> RegistAccount(RegisterView registerView)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Register.cshtml");
            }
            else
            {
                HttpResponseMessage response = await Calculate.callPostApi("Register", registerView);
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


        public IActionResult ForgotPass(string error)
        {
            ViewData["error"] = error;
            return View("~/Views/ForgotPass.cshtml");
        }

        public async Task<IActionResult> ResetPassword(EmailView view)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri("http://localhost:5000/api/");
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.PostAsJsonAsync("ForgotPassword?email=" + view.Email, "");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("ForgotPass", "Permission", new { error = "Email doesn't exist!" });
                }
            }
        }

        public IActionResult ChangePass(string error)
        {
            ViewData["error"] = error;
            return View("~/Views/Customer/ChangePass.cshtml");
        }

        public async Task<IActionResult> ChangeAccountPassword(ChangePassView changePass)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(ChangePass));
                }
                else if (!changePass.NewPassword.Equals(changePass.ConfirmPassword))
                {
                    return RedirectToAction(nameof(ChangePass), new { error = "New password doesn't match confirm password!" });
                }
                else
                {
                    string token = HttpContext.Session.GetString(Constants._accessToken);

                    HttpResponseMessage response = await Calculate.callPostApi("ChangePass", token, changePass);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Info", "Customer");
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        ClaimView claim = await Calculate.GetAccountClaims(token);
                        string refreshToken = HttpContext.Session.GetString(Constants._refreshToken);
                        TokenView tokenView = await Calculate.GetRefreshToken(claim.AccountId, refreshToken);
                        if (tokenView != null)
                        {
                            HttpContext.Session.SetString(Constants._accessToken, tokenView.AccessToken);
                            HttpContext.Session.SetString(Constants._refreshToken, tokenView.RefreshToken);
                            return RedirectToAction(nameof(ChangeAccountPassword), changePass);
                        }
                        else
                        {
                            return RedirectToAction("Login", "Permission");
                        }
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        return RedirectToAction(nameof(ChangePass), new { error = error });
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }

        }

    }
}

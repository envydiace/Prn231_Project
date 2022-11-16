using ClientApp.Models;
using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ClientApp.Controllers
{
    public class CustomerController : Controller
    {
        public async Task<IActionResult> Info()
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callGetApi("Customer/GetCustomer", token);
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    CustomerInfoView cus = JsonConvert.DeserializeObject<CustomerInfoView>(results);
                    ViewData["customerInfo"] = cus;
                    HttpContext.Session.SetString(Constants._cusName, cus.ContactName);
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
                        return RedirectToAction(nameof(Info));
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }

        }

        public async Task<IActionResult> Edit()
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callGetApi("Customer/GetCustomer", token);
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    CustomerEditView cus = JsonConvert.DeserializeObject<CustomerEditView>(results);
                    HttpContext.Session.SetString(Constants._cusName, cus.ContactName);
                    return View("~/Views/Customer/Edit.cshtml", cus);
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
                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
        }

        public async Task<IActionResult> EditProfile(CustomerEditView customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string token = HttpContext.Session.GetString(Constants._accessToken);

                    HttpResponseMessage response = await Calculate.callPutApi("Customer/EditCustomerInfo", token, customer);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Info));
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
                            return RedirectToAction(nameof(EditProfile), customer);
                        }
                        else
                        {
                            return RedirectToAction("Login", "Permission");
                        }
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Edit));
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
        }
        public async Task<IActionResult> Order()
        {
            string token = HttpContext.Session.GetString(Constants._accessToken);
            HttpResponseMessage response = await Calculate.callGetApi("Order/GetCustomerOrder", token);

            if (response.IsSuccessStatusCode)
            {
                string results = response.Content.ReadAsStringAsync().Result;
                List<OrderView> orders = JsonConvert.DeserializeObject<List<OrderView>>(results);
                ViewData["orders"] = orders;
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
                    return RedirectToAction(nameof(Order));
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
            {
                return RedirectToAction("Login", "Permission");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            return View();
        }

        public async Task<IActionResult> OrderCanceled()
        {

            string token = HttpContext.Session.GetString(Constants._accessToken);
            HttpResponseMessage response = await Calculate.callGetApi("Order/GetCustomerCanceledOrder", token);

            if (response.IsSuccessStatusCode)
            {
                string results = response.Content.ReadAsStringAsync().Result;
                List<OrderView> orders = JsonConvert.DeserializeObject<List<OrderView>>(results);
                ViewData["orders"] = orders;
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
                    return RedirectToAction(nameof(OrderCanceled));
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
            {
                return RedirectToAction("Login", "Permission");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            return View("~/Views/Customer/Order.cshtml");
        }

    }
}

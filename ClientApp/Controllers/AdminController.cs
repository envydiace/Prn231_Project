using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using ClientApp.Models;
using Newtonsoft.Json;
using ClientApp.Utils;
using System.Diagnostics;
using OfficeOpenXml;

namespace ClientApp.Controllers
{
    public class AdminController : Controller
    {
        public async Task<ActionResult> Product(ProductSearchView? searchView)
        {
            try
            {
                var categories = await Calculate.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage productResponse = await Calculate.callGetApi("Product/GetAllFilter?categoryId=" + searchView.CategoryId + (searchView.Search != null ? ("&search=" + searchView.Search) : ""), token);

                if (productResponse.IsSuccessStatusCode)
                {
                    string results = productResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> products = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["products"] = products;

                }
                else if (productResponse.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    ClaimView claim = await Calculate.GetAccountClaims(token);
                    string refreshToken = HttpContext.Session.GetString(Constants._refreshToken);
                    if (refreshToken == null || claim == null)
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    TokenView tokenView = await Calculate.GetRefreshToken(claim.AccountId, refreshToken);
                    if (tokenView != null)
                    {
                        HttpContext.Session.SetString(Constants._accessToken, tokenView.AccessToken);
                        HttpContext.Session.SetString(Constants._refreshToken, tokenView.RefreshToken);
                        return RedirectToAction(nameof(Product), searchView);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else if (productResponse.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {

                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
                return View("~/Views/Admin/Index.cshtml");
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
        }

        public async Task<ActionResult> Create()
        {
            try
            {
                var categories = await Calculate.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Create.cshtml");
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
            
        }

        public async Task<ActionResult> Update(int id)
        {
            try
            {
                var categories = await Calculate.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Update.cshtml", await GetProductById(id));
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
            
        }

        public async Task<ActionResult> CreateProduct(ProductAdd product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string token = HttpContext.Session.GetString(Constants._accessToken);

                    HttpResponseMessage response = await Calculate.callPostApi("Product/Create", token, product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
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
                            return RedirectToAction(nameof(CreateProduct), product);
                        }
                        else
                        {
                            return RedirectToAction("Login", "Permission");
                        }
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }

                }
                else
                {
                    var categories = await Calculate.GetAllCategory();
                    if (categories == null)
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    ViewData["categories"] = categories;
                    return View("~/Views/Admin/Create.cshtml");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }

        }

        public async Task<ActionResult> UpdateProduct(ProductEdit product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string token = HttpContext.Session.GetString(Constants._accessToken);
                    HttpResponseMessage response = await Calculate.callPutApi("Product/Update", token, product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
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
                            return RedirectToAction(nameof(UpdateProduct), product);
                        }
                        else
                        {
                            return RedirectToAction("Login", "Permission");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else
                {
                    var categories = await Calculate.GetAllCategory();
                    if (categories == null)
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    ViewData["categories"] = categories;
                    return View("~/Views/Admin/Update.cshtml");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
            
        }

        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callDeleteApi($"Product/Delete/{id}", token);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Product));
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Index", "Home");
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
        public async Task<ActionResult> Order(OrderSearchView? searchView)
        {
            try
            {
                string param = "";
                if (searchView.From != null)
                {
                    param += "&from=" + DateTime.Parse(searchView.From.ToString()).ToString("yyyy-MM-dd");
                }
                if (searchView.To != null)
                {
                    param += "&to=" + DateTime.Parse(searchView.To.ToString()).ToString("yyyy-MM-dd");
                }

                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callGetApi("Order/GetAllOrders?pageIndex=" + searchView.page + "&pageSize=20" + param, token);

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    OrderPagingView orders = JsonConvert.DeserializeObject<OrderPagingView>(results);
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
                        return RedirectToAction(nameof(Order), searchView);
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

                ViewData["searchView"] = searchView;
                return View(searchView);
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Permission", new { error = Constants.ErrorMessage.SomeThingHappend });
            }
            
        }

        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callPutApi("Order/CancelOrder?orderId=" + id, token, id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Order));
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
                        return RedirectToAction(nameof(CancelOrder), id);
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
        public async Task<ActionResult> OrderDetail(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage response = await Calculate.callGetApi("Order/GetOrderDetail?id=" + id, token);

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    OrderView orderDetail = JsonConvert.DeserializeObject<OrderView>(results);
                    ViewData["orderDetail"] = orderDetail;
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
                        return RedirectToAction(nameof(OrderDetail), id);
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

        public async Task<IActionResult> Dashboard(int year)
        {
            try
            {
                string token = HttpContext.Session.GetString(Constants._accessToken);
                HttpResponseMessage dashboardResponse = await Calculate.callGetApi("Dashboard/GetDashboard", token);
                if (dashboardResponse.IsSuccessStatusCode)
                {
                    string results = dashboardResponse.Content.ReadAsStringAsync().Result;
                    DashboardView dashboard = JsonConvert.DeserializeObject<DashboardView>(results);
                    ViewData["dashboard"] = dashboard;

                }
                else if (dashboardResponse.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    ClaimView claim = await Calculate.GetAccountClaims(token);
                    string refreshToken = HttpContext.Session.GetString(Constants._refreshToken);
                    TokenView tokenView = await Calculate.GetRefreshToken(claim.AccountId, refreshToken);
                    if (tokenView != null)
                    {
                        HttpContext.Session.SetString(Constants._accessToken, tokenView.AccessToken);
                        HttpContext.Session.SetString(Constants._refreshToken, tokenView.RefreshToken);
                        return RedirectToAction(nameof(Dashboard), year);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }

                HttpResponseMessage response = await Calculate.callGetApi("Dashboard/GetStaticOrder?year=" + year, token);
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    List<int> orders = JsonConvert.DeserializeObject<List<int>>(results);
                    ViewData["orders"] = results;

                }
                else if (dashboardResponse.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    ClaimView claim = await Calculate.GetAccountClaims(token);
                    string refreshToken = HttpContext.Session.GetString(Constants._refreshToken);
                    TokenView tokenView = await Calculate.GetRefreshToken(claim.AccountId, refreshToken);
                    if (tokenView != null)
                    {
                        HttpContext.Session.SetString(Constants._accessToken, tokenView.AccessToken);
                        HttpContext.Session.SetString(Constants._refreshToken, tokenView.RefreshToken);
                        return RedirectToAction(nameof(Dashboard), year);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
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

        private async Task<ProductEdit> GetProductById(int id)
        {
            HttpResponseMessage productResponse = await Calculate.callGetApi($"Product/Get/{id}");
            if (productResponse.IsSuccessStatusCode)
            {
                string results = productResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ProductEdit>(results);
            }
            else
            {
                Console.WriteLine("Error Calling web API");
                return null;
            }
        }

        public async Task<ActionResult> ImportExcelFile(IFormFile file)
        {
            try
            {
                var product = new List<ProductAdd>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            product.Add(new ProductAdd
                            {
                                ProductName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                CategoryId = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString().Trim()),
                                QuantityPerUnit = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                UnitsInStock = short.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                UnitsOnOrder = short.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                ReorderLevel = short.Parse(worksheet.Cells[row, 7].Value.ToString().Trim()),
                                Discontinued = Convert.ToBoolean(worksheet.Cells[row, 8].Value.ToString().Trim())
                            });
                        }
                        ViewData["excelList"] = product;
                    }

                    HttpResponseMessage response = await Calculate.callPostApi("Product/CreateMulti", product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
                    }
                    else
                    {
                        ViewData["error"] = "Import product fail!";
                        return View("~/Views/Admin/Error.cshtml");
                    }

                }
            }
            catch (Exception)
            {
                ViewData["error"] = "Import product fail!";
                return View("~/Views/Admin/Error.cshtml");
            }

        }

    }
}

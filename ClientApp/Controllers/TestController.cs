using ClientApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    public class TestController : Controller
    {
        public async Task< IActionResult >Index()
        {
            var token = HttpContext.Session.GetString(Constants._accessToken);
            ViewData["claim"] =await Calculate.GetAccountClaims(token);
            return View("~/Views/Test.cshtml");
        }
    }
}

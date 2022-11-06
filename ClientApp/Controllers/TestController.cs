using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index(string? token)
        {
            ViewData["token"] = token;
            return View("~/Views/Test.cshtml");
        }
    }
}

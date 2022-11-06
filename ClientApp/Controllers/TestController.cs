using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            ViewData["token"] = HttpContext.Session.GetString("token");
            return View("~/Views/Test.cshtml");
        }
    }
}

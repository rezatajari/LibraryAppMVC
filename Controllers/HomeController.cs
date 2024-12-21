using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

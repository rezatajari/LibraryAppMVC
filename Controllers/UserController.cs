using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

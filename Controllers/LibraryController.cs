using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Remove()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

    }
}

using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryAppMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccountDeleted()
        {
            return View();
        }

        [HttpGet(template: "Home/Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
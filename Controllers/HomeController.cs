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

        [HttpGet(template: "Home/Error")]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            Console.WriteLine($"Error: {exception?.Error.Message}");

            return View("Error", new ErrorViewModel
            {
                Title = "An unexpected error occurred!",
                Detail = "We are working to fix this issue. Please try again later.",
                StatusCode = 500, // Internal Server Error });   
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
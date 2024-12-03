using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IBookService _bookService;
        public LibraryController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("library/Add")]
        [HttpPost]
        public IActionResult Add(Book newBook)
        {
            if (!ModelState.IsValid)
            {
                return View(newBook);
            }

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

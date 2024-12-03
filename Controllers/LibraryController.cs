using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Services;
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

        [Route("library/home")]
        public IActionResult Home()
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

            _bookService.Add(newBook);

            return View();
        }

        [Route("library/remove")]
        [HttpPost]
        public IActionResult Remove(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            _bookService.Remove(book);

            return View();
        }

        [Route("library/list")]
        [HttpGet]
        public IActionResult List()
        {
            List<Book> books = _bookService.GetAll();
            return View(books);
        }

        [Route("library/searchbytitle/{title}")]
        [HttpGet]
        public IActionResult SearchById(string title)
        {
            List<Book> books = _bookService.SearchByTitle(title);

            return View(books);
        }
    }
}

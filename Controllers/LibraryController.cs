using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Services;
using LibraryAppMVC.ViewModels;
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
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Route("library/Add")]
        [HttpPost]
        public IActionResult Add(AddBookViewModel newBook)
        {
            if (!ModelState.IsValid)
            {
                return View(newBook);
            }

            Book book = new Book()
            {
                Title = newBook.Title,
                Author = newBook.Author,
                Genre = newBook.Genre,
            };

            _bookService.Add(book);

            TempData["SuccessMessage"] = "Book added successfully!";
            return RedirectToAction("Add");
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

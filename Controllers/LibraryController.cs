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
        public async Task<IActionResult> Add(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in!";
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    await _bookService.Add(userId.Value, model);

                    TempData["SuccessMessage"] = "Book added successfully!";
                    return RedirectToAction("Add");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }

            return View(model);
        }

        [Route("library/remove")]
        [HttpPost]
        public async Task<IActionResult> Remove(string title)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            }

            var book = _bookService.SearchByTitle(title).Result;

            if (book == null)
            {
                TempData["ErrorMessage"] = "This book is not exist";
                return RedirectToAction("List");
            }
            await _bookService.Remove(book);
            TempData["RemoveMessage"] = "Book Removed!";
            return RedirectToAction("Home");

        }

        [Route("library/list")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            };

            var books = await _bookService.GetAll(userId);
            var listBookModel = new List<ListBookViewModel>();

            listBookModel = books.Select(b => new ListBookViewModel
            {
                Title = b.Title,
                Author = b.Author,
                Genre = (ListBookViewModel.GenreType)b.Genre
            }).ToList();

            return View(listBookModel);
        }

        [HttpGet]
        [Route("Library/Search")]
        public IActionResult Search()
        {
            return View();
        }

        [Route("library/SearchByTitle")]
        [HttpPost]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            };

            var book = await _bookService.SearchByTitle(title);
            if (book == null)
            {
                TempData["ErrorMessage"] = "This book is not exist";
                return RedirectToAction("Search");
            }

            var bookModel = new BookViewModel()
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre
            };

            return RedirectToAction("BookDetails", bookModel);
        }


        [HttpGet]
        [Route("Library/BookDetails")]
        public IActionResult BookDetails(BookViewModel book)
        {
            return View(book);
        }

    }
}

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

            await _bookService.Remove(title);

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

            var books = await _bookService.GetAll();
            return View(books);
        }

        [Route("library/searchbytitle/{title}")]
        [HttpGet]
        public async Task<IActionResult> SearchById(string title)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            };

            var books = await _bookService.SearchByTitle(title);

            return View(books);
        }
    }
}

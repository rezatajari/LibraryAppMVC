using System.Security.Claims;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAccountService _accountService;
        public LibraryController(IBookService bookService, IAccountService accountService)
        {
            _bookService = bookService;
        }

        [Route(template: "library/home"), HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [Route(template: "library/Add"), HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Route(template: "library/Add"), HttpPost]
        public async Task<IActionResult> Add(BookViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Get current userId
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", controllerName: "Account");
            }

            // Add operation
            var result = await _bookService.Add(model, userId);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Book added successfully!";
                return RedirectToAction("Home");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Add book process is failed");
            return View(model);
        }

        [Route(template: "library/remove"), HttpPost]
        public async Task<IActionResult> Remove(BookViewModel model)
        {
            // Get current userId
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            }

            var result = await _bookService.Remove(model, userId);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("List");
            }

            TempData["RemoveMessage"] = "Book Removed!";
            return RedirectToAction("List");
        }

        [Route(template: "library/list"), HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", controllerName: "Account");
            };

            var result = await _bookService.GetAll(userId);
            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View();
        }

        [HttpGet, Route(template: "Library/Search")]
        public IActionResult Search()
        {
            return View();
        }

        [Route(template: "library/SearchByTitle"), HttpPost]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in!";
                return RedirectToAction("Login", "Account");
            };

            var result = await _bookService.SearchByTitle(title, userId);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "This book is not exist";
                return RedirectToAction("Search");
            }

            return RedirectToAction("BookDetails", result.Data);
        }

        [Route(template: "Library/Delete/{title}"), HttpPost]
        public async Task<IActionResult> Delete(string title)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not logged in";
                return RedirectToAction(actionName: "login", controllerName: "Account");
            }

            var result = await _bookService.Delete(userId, title);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Can not delete your book";
                return RedirectToAction("Search");
            }

            TempData["Successfully"] = "Your book deleted";
            return RedirectToAction("Search");

        }

        [HttpGet, Route(template: "Library/BookDetails")]
        public IActionResult BookDetails(BookViewModel book)
        {
            return View(book);
        }
    }
}

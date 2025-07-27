using System.Security.Claims;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Repositories;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class LibraryController(IBookService bookService) : Controller
    {
        [HttpGet(template: "library/home")]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet(template: "library/Add")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost(template: "library/Add")]
        public async Task<IActionResult> Add(BookViewModel model)
        {
            try
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
                var result = await bookService.Add(model, userId);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Book added successfully!";
                    return RedirectToAction("Home");
                }

                ModelState.AddModelError(string.Empty, errorMessage: result.ErrorMessage ?? "Add failed");
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }

        [HttpPost(template: "library/remove")]
        public async Task<IActionResult> Remove(BookViewModel model)
        {
            try
            {
                // Get current userId
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in!";
                    return RedirectToAction("Login", "Account");
                }

                var result = await bookService.Remove(model, userId);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                    return RedirectToAction("List");
                }

                TempData["RemoveMessage"] = "Book Removed!";
                return RedirectToAction("List");
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }

        [HttpGet(template: "library/list")]
        public async Task<IActionResult> List()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in!";
                    return RedirectToAction("Login", controllerName: "Account");
                }

                var result = await bookService.GetAll(userId);
                if (result.Succeeded)
                    return View(result.Data);

                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }

        [HttpGet, Route(template: "Library/Search")]
        public IActionResult Search()
        {
            return View();
        }
        [HttpPost(template: "library/SearchByTitle")]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in!";
                    return RedirectToAction("Login", "Account");
                }

                var result = await bookService.SearchByTitle(title, userId);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "This book is not exist";
                    return RedirectToAction("Search");
                }

                return RedirectToAction("BookDetails", result.Data);
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }

        [HttpPost(template: "Library/Delete/{title}")]
        public async Task<IActionResult> Delete(string title)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in";
                    return RedirectToAction(actionName: "login", controllerName: "Account");
                }

                var result = await bookService.Delete(userId, title);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Can not delete your book";
                    return RedirectToAction("Search");
                }

                TempData["Successfully"] = "Your book deleted";
                return RedirectToAction("Search");
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }

        }

        [HttpGet(template: "Library/BookDetails/{id}")]
        public async Task<IActionResult> BookDetails(int id)
        {
            try
            {
                var result = await bookService.GetByIdAsync(id);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Your book is not exist";
                    return RedirectToAction("List");
                }

                var model = new BookViewModel
                {
                    Id = result.Data.Id,
                    Title = result.Data.Title,
                    Author = result.Data.Author,
                    Genre = result.Data.Genre
                };

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // 1. Get the book from your data store
                var response = await bookService.GetByIdAsync(id);
                if (!response.Succeeded)
                {
                    TempData["ErrorMessage"] = "This book is not exist";
                    return RedirectToAction("List");
                }

                // 2. Map to ViewModel
                var viewModel = new BookViewModel
                {
                    Id = response.Data.Id,
                    Title = response.Data.Title,
                    Author = response.Data.Author,
                    Genre = response.Data.Genre
                    // Map any additional fields
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> Edit(int id, BookViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("Book ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please correct the errors below";
                    return View(model);
                }

                try
                {
                    // 1. Get existing book
                    var existingBook = await bookService.GetByIdAsync(id);
                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    // 2. Update properties
                    existingBook.Data.Title = model.Title;
                    existingBook.Data.Author = model.Author;
                    existingBook.Data.Genre = model.Genre;
                    // Update additional fields as needed

                    // 3. Save changes
                    var updateResult = await bookService.UpdateAsync(existingBook.Data);

                    if (!updateResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Failed to update book. Please try again.";
                        return View(model);
                    }

                    TempData["SuccessMessage"] = $"'{model.Title}' has been updated successfully";
                    return RedirectToAction("BookDetails", new { id = model.Id });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving. Please try again.";
                    return View(model);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(controllerName: "home", actionName: "Error");
            }
        }
    }
}

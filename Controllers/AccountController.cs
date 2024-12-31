using System.Security.Claims;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace LibraryAppMVC.Controllers
{
    public class AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger,
        IEmailSender emailSender)
        : Controller
    {

        //------------------ Account Section ------------------//
        [HttpGet, Route(template: "Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, Route(template: "Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await accountService.LogIn(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(key: string.Empty, result.ErrorMessage);
                logger.LogError("{Email} at {Time} has {Error}", model.Email, DateTime.UtcNow, result.ErrorMessage);
                return View(model);
            }

            logger.LogInformation("{Email} logged in successfully at {Time}.", model.Email, DateTime.UtcNow);
            return RedirectToAction("Profile");
        }

        [HttpGet, Route(template: "Account/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost, Route(template: "Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await accountService.Registration(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["Registered"] = "Registration successful! A confirmation email has been sent to your email address." +
                " Please confirm your email to activate your account.";
            return RedirectToAction("Login");
        }

        [HttpGet, Route(template: "Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await accountService.ConfirmationEmailProcess(userId, token);

            return !result.Succeeded ? RedirectToAction(actionName: "Error", controllerName: "Home") :
                RedirectToAction("ConfirmEmailSuccess");
        }

        [HttpGet]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }

        [HttpGet, Route(template: "Account/DeleteAccount/{email}")]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var result = await accountService.DeleteAccount(email);

            if (result.Succeeded) return RedirectToAction("AccountDeleted");

            TempData["ErrorMessage"] = "Failed to deleted your account";
            return RedirectToAction(actionName: "Error", controllerName: "Home");
        }

        [HttpGet]
        public IActionResult AccountDeleted()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        //------------------ Profile Section ------------------//
        [HttpGet, Route(template: "Account/Profile")]
        public async Task<IActionResult> Profile(string email)
        {
            var result = await accountService.GetUserByEmail(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View();
        }

        [HttpGet, Route(template: "Account/EditProfile")]
        public async Task<IActionResult> EditProfile(string email)
        {
            var result = await accountService.GetUserByEmail(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View();
        }

        [HttpPost, Route(template: "Account/EditProfile")]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await accountService.EditProfileUser(model, currentUserId);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(model);
                }

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }


        //TODO: Jwt or other best practice stragtegy R&D after implement
    }
}

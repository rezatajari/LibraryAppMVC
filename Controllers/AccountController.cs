using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace LibraryAppMVC.Controllers
{
    public class AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger,
        IEmailSender emailSender)
        : Controller
    {

        [HttpGet, Route(template: "Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, Route(template: "Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, errorMessage) = await accountService.Login(model);
                if (errorMessage != null)
                {
                    ModelState.AddModelError(key: string.Empty, errorMessage);
                    logger.LogError("{Email} at {Time} has {Error}", model.Email, DateTime.UtcNow, errorMessage);
                    return View(model);
                }

                if (result.Succeeded)
                {
                    logger.LogInformation("{Email} logged in successfully at {Time}.", model.Email, DateTime.UtcNow);
                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError(key: string.Empty, errorMessage: "Invalid login attempt.");
                logger.LogError("{Email} at {Time} has {Error}", model.Email, DateTime.UtcNow, "Invalid login attempt.");
                return View(model);
            }
            return View(model);
        }

        [HttpGet, Route(template: "Account/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost, Route(template: "Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, errorMessage) = await accountService.Register(model);
                if (errorMessage != null)
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(model);
                }

                if (result.Succeeded)
                {
                    var confirmationLink = await accountService.GenerateEmailConfirmationLink(model.Email);

                    await emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your email by clicking the following link: <a href='{confirmationLink}'>Confirm Email</a>");

                    return RedirectToAction("RegistrationConfirmationNotice");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet, Route(template: "Account/RegistrationConfirmationNotice")]
        public IActionResult RegistrationConfirmationNotice()
        {
            return View();
        }

        [HttpGet, Route(template: "Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            var (isValid, user) = await accountService.TryConfirmationProcess(userId, token);

            if (!isValid)
            {
                return RedirectToAction(actionName: "Error", controllerName: "Home");
            }

            var result = await accountService.ConfirmEmail(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmEmailSuccess");
            }

            return RedirectToAction(actionName: "Error", controllerName: "Home");
        }

        [HttpGet]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }

        [HttpGet, Route(template: "Account/Profile")]
        public async Task<IActionResult> Profile(string email)
        {
            var user = await accountService.GetUser();

            var profileModel = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return View(profileModel);
        }

        [HttpGet, Route(template: "Account/EditProfile")]
        public async Task<IActionResult> EditProfile(string email)
        {
            var (profileModel, errorMessage) = await accountService.GetUserProfile(email);
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }

            return View(profileModel);
        }

        [HttpPost, Route(template: "Account/EditProfile")]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await accountService.EmailEditExist(model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already in use.");
                    return View(model);
                }

                await accountService.EditProfileUser(model);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }


        [HttpGet, Route(template: "Account/DeleteAccount/{email}")]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var (success, errorMessage) = await accountService.DeleteAccount(email);

            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(actionName: "Error", controllerName: "Home");
            }

            await HttpContext.SignOutAsync();

            return RedirectToAction("AccountDeleted");
        }

        public IActionResult AccountDeleted()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

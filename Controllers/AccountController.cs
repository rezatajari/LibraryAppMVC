using Azure.Identity;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Services;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger,
                                 IEmailSender emailSender)
        {
            _accountService = accountService;
            _logger = logger;
            _emailSender = emailSender;
        }

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

                var result = await _accountService.Login(model);

                if (result.Succeeded)
                    return RedirectToAction("Profile");

                ModelState.AddModelError(key: string.Empty, errorMessage: "Invalid login attempt.");
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
                var result = await _accountService.Register(model);

                if (result.Succeeded)
                {
                    // Generate an email confirmation process
                    var confirmationLink = await _accountService.GenerateEmailConfirmationLink(model.Email);

                    // Send confirmation email
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your email by clicking the following link: <a href='{confirmationLink}'>Confirm Email</a>");

                    // Custom notice page
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

            var (isValid, user) = await _accountService.TryConfirmationProcess(userId, token);

            if (!isValid)
            {
                return RedirectToAction(actionName: "Error", controllerName: "Home");
            }

            var result = await _accountService.ConfirmEmail(user, token);
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
        public async Task<IActionResult> Profile(User user)
        {
            var profileModel = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return View(profileModel);
        }

        [HttpGet, Route(template: "Account/EditProfile/{email}")]
        public async Task<IActionResult> EditProfile(string email)
        {
            var profileModel = await _accountService.ProfileEditPage(email);
            return View(profileModel);
        }

        [HttpPost, Route(template: "Account/EditProfile")]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _accountService.EmailEditExist(model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already in use.");
                    return View(model);
                }

                await _accountService.EditProfileUserByEmail(model);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }


        [HttpGet, Route(template: "Account/DeleteAccount/{email}")]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var (success, errorMessage) = await _accountService.DeleteAccount(email);

            if (!success)
            {
                TempData["ErrorMessage"] = "Somethings wronged!!";
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

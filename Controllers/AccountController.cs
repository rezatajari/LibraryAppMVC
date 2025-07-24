using System.Security.Claims;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAppMVC.Controllers
{
    [Route(template:"[controller]")]
    public class AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger)
        : Controller
    {
        #region Login

        [HttpGet(template: "[action]")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost(template: "[action]")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await accountService.LogIn(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(key: string.Empty, errorMessage: result.ErrorMessage ?? "Login failed!");
                logger.LogError("{Email} at {Time} has {Error}", model.Email, DateTime.UtcNow, result.ErrorMessage);
                return View(model);
            }

            logger.LogInformation("{Email} logged in successfully at {Time}.", model.Email, DateTime.UtcNow);
            return RedirectToAction("Profile",routeValues:new {email=model.Email});
        }

        #endregion

        #region Register

        [HttpGet(template: "[action]")]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost(template: "[action]")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await accountService.Registration(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(key:string.Empty, result.ErrorMessage ?? "Registration is failed");
                return View(model);
            }

            TempData["Registered"] = "Registration successful! A confirmation email has been sent to your email address." +
                                     " Please confirm your email to activate your account.";
            return RedirectToAction("Login");
        }

        #endregion

        [HttpGet(template: "[action]")]
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

        //------- Delete Account Section -------//
        [HttpGet(template: "[action]/{email}")]
        public async Task<IActionResult> DeleteAccount(string email)
        {
            var result = await accountService.DeleteAccount(email);

            if (result.Succeeded) return RedirectToAction("AccountDeleted");

            TempData["ErrorMessage"] = "Failed to deleted your account";
            return RedirectToAction(actionName: "Error", controllerName: "Home");
        }
        [HttpGet(template:"[action]")]
        public IActionResult AccountDeleted()
        {
            return View();
        }

        //------- Logout Section -------//
        [HttpGet(template: "[action]")]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        //================================ Profile Section ================================//

        //------- Profile Page Section -------//
        [Authorize]
        [HttpGet(template: "[action]")]
        public async Task<IActionResult> Profile()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var result = await accountService.GetUserByEmail(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
            return View(new ProfileViewModel());
        }

        //------- Edit Profile Section -------//
        [Authorize]
        [HttpGet(template: "[action]")]
        public async Task<IActionResult> EditProfile(string email)
        {
            var result = await accountService.GetUserByEmail(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
            return View();
        }
        [Authorize]
        [HttpPost(template: "[action]")]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                ModelState.AddModelError(string.Empty, "User is not exist");
                return View(model);
            }

            var result = await accountService.EditProfileUser(model, currentUserId);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}

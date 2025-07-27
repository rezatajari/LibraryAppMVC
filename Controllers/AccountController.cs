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
            return RedirectToAction(controllerName:"Library",actionName:"Home",routeValues:new {email=model.Email});
        }

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

        [Authorize]
        [HttpGet(template:"[action]")]
        public IActionResult DeleteConfirmation()
        {
            return View();
        }

        [Authorize]
        [HttpPost(template: "[action]")]
        public async Task<IActionResult> Delete()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var result = await accountService.DeleteAccount(email);

            if (result.Succeeded) return RedirectToAction(controllerName:"Home",actionName:"AccountDeleted");

            TempData["ErrorMessage"] = "Failed to deleted your account";
            return RedirectToAction(actionName: "Profile", controllerName: "Profile");
        }

        [HttpGet(template: "[action]")]
        public async Task<IActionResult> Logout()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

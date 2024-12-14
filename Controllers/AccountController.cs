using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Services;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using NuGet.Packaging.Signing;

namespace LibraryAppMVC.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountService.Login(model);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    TempData["SuccessMessage"] = "Login successful!";

                    var profile = new ProfileViewModel()
                    {
                        Email = user.Email,
                        UserName = user.UserName
                    };
                    return RedirectToAction("Profile", profile);
                }

                TempData["ErrorMessage"] = "Invalid login credentials. Please try again.";
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [Route("Account/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool userExist = await _accountService.CheckUserExist(model.Email);
                if (userExist)
                {
                    TempData["ErrorMessage"] = "A user with this email already exists.";
                    return View(model);
                }

                await _accountService.Register(model);
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }

            return View(model);

        }

        [HttpGet]
        [Route("Account/Profile")]
        public IActionResult Profile(ProfileViewModel model)
        {
            return View(model);
        }
    }
}

using LibraryAppMVC.Interfaces;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

                    return RedirectToAction("Profile");
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
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user =await _accountService.GetUserById(userId);

            var profileModel = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return View(profileModel);
        }

        [HttpGet]
        [Route("Account/EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = await _accountService.GetUserById(userId);

            var profileModel = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return View(profileModel);
        }


        [HttpPost]
        [Route("Account/EditProfile")]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                var emailExists = await _accountService.EmailEditExist(userId, model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already in use.");
                    return View(model);
                }

                await _accountService.EditProfileUser(userId, model);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }


        [HttpGet]
        [Route("Account/DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var result = await _accountService.RemoveUser(userId);

            if (result)
            {

                TempData["SuccessMessage"] = "Profile Deleted";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Somethings wronge!!";
            return RedirectToAction("Profile");
        }

    }
}

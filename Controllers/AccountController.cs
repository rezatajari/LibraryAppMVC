using LibraryAppMVC.Interfaces;
using LibraryAppMVC.ViewModels;
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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger,
                                 SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
                                 IEmailSender emailSender)
        {
            _accountService = accountService;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
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
                _logger.LogInformation("Valid email & password user: {email}", model.Email);
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

                var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Generate an email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Create the confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                    //TODO:Alternative: Use Fake SMTP for Testing
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

        [HttpGet]
        [Route("Account/RegistrationConfirmationNotice")]
        public IActionResult RegistrationConfirmationNotice()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmEmailSuccess");
            }

            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }

        [HttpGet]
        [Route("Account/Profile")]
        public async Task<IActionResult> Profile()
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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

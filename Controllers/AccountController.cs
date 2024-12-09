using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Services;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
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
                bool login = _accountService.Login(model);

                if (login)
                {
                    return RedirectToAction("Library", "Home");
                }

                return View();
            }

            return View(model);
        }
    }
}

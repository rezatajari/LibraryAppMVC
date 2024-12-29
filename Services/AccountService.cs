using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NuGet.Protocol;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LibraryAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<BookService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(IAccountRepository accountRepository, ILogger<BookService> logger,
                UserManager<User> userManager, SignInManager<User> signInManager,
                IUrlHelperFactory helperFactory, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            var actionContext = new ActionContext(
                _httpContextAccessor.HttpContext,
                _httpContextAccessor.HttpContext.GetRouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            _urlHelper = helperFactory.GetUrlHelper(actionContext);
        }


        public async Task<SignInResult> Login(LoginViewModel model)
        {
            var user = await FindUserLoginByEmail(model.Email);
            CheckEmailConfirmation(user);

            var result = await _signInManager.PasswordSignInAsync(userName: user.UserName, password: model.Password,
                isPersistent: true, lockoutOnFailure: false);
            return result;
        }


        public async Task<IdentityResult> Register(RegisterViewModel model)
        {
            FindUserRegisterByEmail(model.Email);

            var user = new User
            {
                BirthdayDate = model.BirthdayDate
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        private async Task FindUserRegisterByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
                throw new Exception("An account with this email already exists.");
        }

        public async Task<string> GenerateEmailConfirmationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return _urlHelper.Action(
                 action: "ConfirmEmail",
                 controller: "Account",
                 new { userId = user.Id, token = token },
                 protocol: _httpContextAccessor.HttpContext.Request.Scheme
             );
        }

        public async Task<(bool Success, User user)> TryConfirmationProcess(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return (false, null);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, null);

            return (true, user);
        }

        public async Task<IdentityResult> ConfirmEmail(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        private bool CheckEmailConfirmation(User user)
        {
            var Check = user is { EmailConfirmed: false };
            if (!Check)
                throw new Exception("Email is not confirmed. Please confirm your email to log in.");

            return Check;
        }

        public async Task<User> FindUserLoginByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User does not exist.");

            return user;
        }

        public async Task<ProfileViewModel> ProfileEditPage(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var profileEdit = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                ExistProfilePicture = user.ProfilePicturePath
            };
            return profileEdit;
        }

        public async Task EditProfileUserByEmail(ProfileViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            user.UserName = model.UserName;
            user.Email = model.Email;

            if (model.ProfilePicture != null)
            {
                // Define the folder to save the file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);

                // Save the file to the folder
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePicturePath = "/uploads/" + fileName;
            }

            await _accountRepository.UpdateUser(user);
        }


        public async Task<bool> EmailEditExist(string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(newEmail);

            return user != null;
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteAccount(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to delete user: {errors}");
            }

            return (true, null);
        }
    }
}

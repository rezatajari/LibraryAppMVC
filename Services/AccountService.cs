using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
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


        public async Task<ResultTask<SignInResult>> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ResultTask<SignInResult>.Failure("User not found!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return ResultTask<SignInResult>.Failure("Email is not confirmed");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password,
                isPersistent: true, lockoutOnFailure: false);

            return ResultTask<SignInResult>.Success(result);
        }

        public async Task<User> GetUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }
        public async Task<(IdentityResult result, string? errorMessage)> Register(RegisterViewModel model)
        {
            var errorMessage = await FindUserRegisterByEmail(model.Email);

            if (errorMessage != null)
                return (null, errorMessage);

            var user = new User
            {
                UserName = model.UserName,
                BirthdayDate = model.BirthdayDate,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return (result, null);
        }

        private async Task<string?> FindUserRegisterByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null ? "An account with this email already exists." : null;
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


        public async Task<(ProfileViewModel profileView, string errorMessage)> GetUserProfile(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (null, "User not found!");

            var profileView = new ProfileViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return (profileView, null);
        }

        public async Task EditProfileUser(ProfileViewModel model)
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
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePicturePath = "/uploads/" + fileName;
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> EmailEditExist(string? newEmail)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _userManager.FindByEmailAsync(newEmail);
            if (user != null && user.Id != currentUserId)
                return true;

            return false;
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

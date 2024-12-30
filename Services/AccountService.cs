using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
using NuGet.Protocol;
using YourProject.Services;
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
        private readonly IEmailSender _emailSender;
        public AccountService(IAccountRepository accountRepository, ILogger<BookService> logger,
                UserManager<User> userManager, SignInManager<User> signInManager,
                IUrlHelperFactory helperFactory, IHttpContextAccessor httpContextAccessor,
                IEmailSender emailSender)
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
            _emailSender = emailSender;
        }


        public async Task<ResultTask<SignInResult>> LogIn(LoginViewModel model)
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
        public async Task<ResultTask<bool>> Registration(RegisterViewModel model)
        {
            // register
            var registerResult = await RegisterUserOnly(model);
            if (!registerResult.Succeeded)
                return registerResult;

            // send email
            var emailResult = await SendEmailConfirmation(model.Email);
            if (!emailResult.Succeeded)
                return ResultTask<bool>.Failure(emailResult.ErrorMessage);

            // registration result
            return ResultTask<bool>.Success(true);

        }
        private async Task<ResultTask<bool>> RegisterUserOnly(RegisterViewModel model)
        {
            var userExist = await _userManager.FindByEmailAsync(model.Email);
            if (userExist != null)
                return ResultTask<bool>.Failure("An account with this email already exists.");

            var user = new User
            {
                UserName = model.UserName,
                BirthdayDate = model.BirthdayDate,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create user {Email} at {Time}: {Errors}",
                    model.Email,
                    DateTime.UtcNow,
                    string.Join(", ",result.Errors.SelectMany(e=>e.Description)));
             return ResultTask<bool>.Failure(result.Errors.FirstOrDefault()?.Description?? "An error occurred while creating the user.");
            }

            _logger.LogInformation("User created successfully for {Email} at {Time}.",model.Email,DateTime.UtcNow);
            return ResultTask<bool>.Success(true);
        }
        private async Task<ResultTask<bool>> SendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _urlHelper.Action(
                action: "ConfirmEmail",
                controller: "Account",
                new { userId = user.Id, token = token },
                protocol: _httpContextAccessor.HttpContext.Request.Scheme
            );

            const string subject = "Confirm your email";
            var message =
                $"Please confirm your email by clicking the following link: <a href='{confirmationLink}'>Confirm Email</a>";
            try
            {
                await _emailSender.SendEmailAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", user.Email);
                return ResultTask<bool>.Failure("Failed to send email.");
            }

            return ResultTask<bool>.Success(true);
        }
        public async Task<ResultTask<bool>> ConfirmationEmailProcess(string userId, string token)
        {
            // Validation userId & token of user
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                const string errorMessage =
                    "Invalid confirmation link. Please ensure you used the correct link sent to your email.";
                _logger.LogError("Email confirmation failed: UserId or Token is null or empty at {Time}.",
                    DateTime.UtcNow);
                return ResultTask<bool>.Failure(errorMessage);
            }

            // Get & validation of user 
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                const string errorMessage = "The user does not exist. Please contact support if this issue persists.";
                _logger.LogError("Email confirmation failed: User not found for UserId {UserId} at {Time}.", userId,
                    DateTime.UtcNow);
                return ResultTask<bool>.Failure(errorMessage);
            }

            // Confirmation email
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                _logger.LogError("Email confirmation failed for UserId {UserId} at {Time}: {Error}",
                    user.Id,
                    DateTime.UtcNow,
                    string.Join(", ", result.Errors.SelectMany(e => e.Description)));
                return ResultTask<bool>.Failure(
                    "We couldn't confirm your email. The link might have expired or is invalid. Please request a new confirmation email.");
            }

            return ResultTask<bool>.Success(true);
        }


        public async Task<User> GetUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
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

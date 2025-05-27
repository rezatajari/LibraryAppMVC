using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LibraryAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly JwtService _jwtService;
        public AccountService(ILogger<AccountService> logger,
                UserManager<User> userManager, SignInManager<User> signInManager,
                IUrlHelperFactory helperFactory, IHttpContextAccessor httpContextAccessor,
                IEmailSender emailSender,
                JwtService jwtService)
        {
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
            _jwtService = jwtService;
        }

        //------------------ Account Services ------------------//
        public async Task<ResultTask<SignInResult>> LogIn(LoginViewModel model)
        {
            // Check email exist
            if (model.Email == null)
                return ResultTask<SignInResult>.Failure("The email of user is null");

            // Check user exist
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ResultTask<SignInResult>.Failure("User not found!");

            // Check user password
            if(string.IsNullOrEmpty(model.Password))
                return ResultTask<SignInResult>.Failure("The password is null or empty");
            var passValidation = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passValidation)
                return ResultTask<SignInResult>.Failure("Your password is wrong");

            // Check email confirmation
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
                    string.Join(", ", result.Errors.SelectMany(e => e.Description)));
                return ResultTask<bool>.Failure(result.Errors.FirstOrDefault()?.Description ?? "An error occurred while creating the user.");
            }

            _logger.LogInformation("User created successfully for {Email} at {Time}.", model.Email, DateTime.UtcNow);
            return ResultTask<bool>.Success(true);
        }
        private async Task<ResultTask<bool>> SendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return ResultTask<bool>.Failure("User is null");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _urlHelper.Action(
                action: "ConfirmEmail",
                controller: "Account",
                new { userId = user.Id, token },
                protocol: _httpContextAccessor.HttpContext?.Request.Scheme
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
                const string? errorMessage =
                    "Invalid confirmation link. Please ensure you used the correct link sent to your email.";
                _logger.LogError("Email confirmation failed: UserId or Token is null or empty at {Time}.",
                    DateTime.UtcNow);
                return ResultTask<bool>.Failure(errorMessage);
            }

            // Get & validation of user 
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                const string? errorMessage = "The user does not exist. Please contact support if this issue persists.";
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
        public async Task<ResultTask<bool>> DeleteAccount(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ResultTask<bool>.Failure("User not found!");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ResultTask<bool>.Failure(string.Join(", ", result.Errors.SelectMany(e => e.Description)));

            return ResultTask<bool>.Success(true);
        }

        //------------------ Profile Services ------------------//
        public async Task<ResultTask<ProfileViewModel>> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ResultTask<ProfileViewModel>.Failure("User is not found!");

            var profile = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return ResultTask<ProfileViewModel>.Success(data: profile);
        }
        public async Task<ResultTask<bool>> EditProfileUser(ProfileViewModel model, string currentUserId)
        {
            // Get current user information
            if (model.Email == null) return ResultTask<bool>.Failure("The email is null");
            var currentUser = await _userManager.FindByEmailAsync(model.Email);
            if (currentUser == null)
                return ResultTask<bool>.Failure("User by this information is not found.");

            // Existing user by another userId with the new email edited
            var emailValidation = currentUser.Id != currentUserId;
            if (emailValidation)
                return ResultTask<bool>.Failure("This email address is already in use.");

            // Set edited new information
            currentUser.UserName = model.UserName;

            // Generate profile picture path and save picture file
            if (model.ProfilePicture != null)
            {
                var fileName = await FileNamePictureProfile(model.ProfilePicture);
                currentUser.ProfilePicturePath = "/uploads/" + fileName;
            }

            var result = await _userManager.UpdateAsync(currentUser);
            if (!result.Succeeded)
                return ResultTask<bool>.Failure(string.Join(", ", result.Errors.SelectMany(e => e.Description)));

            return ResultTask<bool>.Success(true);
        }
        private async Task<string> FileNamePictureProfile(IFormFile profilePicture)
        {
            // Define the folder to save the file
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            // Generate a unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(profilePicture.FileName);

            // Save the file to the folder
            var filePath = Path.Combine(uploadsFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await profilePicture.CopyToAsync(stream);

            return fileName;
        }
    }
}

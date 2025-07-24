using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LibraryAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AccountService(ILogger<AccountService> logger,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                IEmailService emailService
               )
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<ResultTask<bool>> LogIn(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ResultTask<bool>.Failure($"User not found by this email: {model.Email}");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return ResultTask<bool>.Failure("Email is not confirmed");

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password,
                      isPersistent: true, lockoutOnFailure: false);

            return !signInResult.Succeeded
                ? ResultTask<bool>.Failure("Sign in processor is failed!")
                : ResultTask<bool>.Success(true);
        }


        public async Task<ResultTask<bool>> Registration(RegisterViewModel model)
        {

            // register
            var userExist = await _userManager.FindByEmailAsync(model.Email);
            if (userExist != null)
                return ResultTask<bool>.Failure("An account with this email already exists.");

            var user = new User
            {
                UserName = model.UserName,
                BirthdayDate = model.BirthdayDate,
                Email = model.Email
            };

            var userCreateResult = await _userManager.CreateAsync(user, model.Password);
            if (!userCreateResult.Succeeded)
            {
                _logger.LogError("Failed to create user {Email} at {Time}: {Errors}",
                    model.Email,
                    DateTime.UtcNow,
                    string.Join(", ", userCreateResult.Errors.SelectMany(e => e.Description)));
                return ResultTask<bool>.Failure(userCreateResult.Errors.FirstOrDefault()?.Description ?? "An error occurred while creating the user.");
            }
            _logger.LogInformation("User created successfully for {Email} at {Time}.", model.Email, DateTime.UtcNow);


            // send email
            var emailResult = await _emailService.SendEmail(user);
            if (!emailResult.Succeeded)
                return ResultTask<bool>.Failure(emailResult.ErrorMessage);

            // registration result
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

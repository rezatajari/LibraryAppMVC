using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Services
{
    public class ProfileService:IProfileService
    {
        private readonly UserManager<User> _userManager;
        public ProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResultTask<ProfileViewModel>> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ResultTask<ProfileViewModel>.Failure("User is not found!");

            var profile = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                BirthdayDate=user.BirthdayDate,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return ResultTask<ProfileViewModel>.Success(data: profile);
        }

        public async Task<ResultTask<EditProfileViewModel>> GetEditProfile(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ResultTask<EditProfileViewModel>.Failure("User is not found!");

            var editProfile = new EditProfileViewModel
            {
                UserName = user.UserName,
                BirthdayDate = user.BirthdayDate,
                ExistProfilePicture = user.ProfilePicturePath
            };

            return ResultTask<EditProfileViewModel>.Success(data:editProfile);
        }

        public async Task<ResultTask<bool>> EditProfileUser(EditProfileViewModel model, string userId)
        {
            // Get current user information
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ResultTask<bool>.Failure("User by this information is not found.");

            // Set edited new information
            user.UserName = model.UserName;
            user.BirthdayDate = model.BirthdayDate;

            // Generate profile picture path and save picture file
            if (model.ProfilePicture != null)
            {
                var fileName = await FileNamePictureProfile(model.ProfilePicture);
                user.ProfilePicturePath = "/uploads/" + fileName;
            }

            var result = await _userManager.UpdateAsync(user);
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

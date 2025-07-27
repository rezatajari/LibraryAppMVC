using LibraryAppMVC.Services;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LibraryAppMVC.Interfaces;

namespace LibraryAppMVC.Controllers
{
 [Authorize]
    public class ProfileController(IProfileService profileService) : Controller
    {
        private readonly IProfileService _profileService = profileService;

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var result = await profileService.GetUserByEmail(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
            return View(new ProfileViewModel());
        }
        [HttpGet(template: "[action]")]
        public async Task<IActionResult> EditProfile(string email)
        {
            var result = await profileService.GetEditProfile(email);

            if (result.Succeeded)
                return View(result.Data);

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
            return View();
        }
        [HttpPost(template: "[action]")]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "User is not exist");
                return View(model);
            }

            var result = await profileService.EditProfileUser(model, userId);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? string.Empty);
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}

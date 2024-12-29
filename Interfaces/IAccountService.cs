using System.Diagnostics.Eventing.Reader;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        Task<SignInResult> Login(LoginViewModel model);
        Task<bool> EmailEditExist(string newEmail);
        Task<ProfileViewModel> ProfileEditPage(string email);
        Task EditProfileUserByEmail(ProfileViewModel model);
        Task<(bool Success, string ErrorMessage)> DeleteAccount(string email);
        Task<IdentityResult> Register(RegisterViewModel model);
        Task<string> GenerateEmailConfirmationLink(string email);
        Task<(bool Success, User user)> TryConfirmationProcess(string userId, string token);
        Task<IdentityResult> ConfirmEmail(User user, string token);
        string GetCurrentUserId();
    }

}

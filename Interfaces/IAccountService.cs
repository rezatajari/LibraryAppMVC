using System.Diagnostics.Eventing.Reader;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        Task<ResultTask<SignInResult>> LogIn(LoginViewModel model);
        Task<ResultTask<bool>> Registration(RegisterViewModel model);
        Task<ResultTask<bool>> ConfirmationProcess(string userId, string token);
        Task<bool> EmailEditExist(string? newEmail);
        Task<(ProfileViewModel profileView, string errorMessage)> GetUserProfile(string email);
        Task EditProfileUser(ProfileViewModel model);
        Task<(bool Success, string ErrorMessage)> DeleteAccount(string email);
        string GetCurrentUserId();
        Task<User> GetUser();
    }

}

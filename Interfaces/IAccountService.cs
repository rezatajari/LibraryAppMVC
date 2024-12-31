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
        Task<ResultTask<bool>> ConfirmationEmailProcess(string userId, string token);
        Task<ResultTask<bool>> DeleteAccount(string email);
        Task<bool> EmailEditExist(string? newEmail);
        Task<(ProfileViewModel profileView, string errorMessage)> GetUserProfile(string email);
        Task EditProfileUser(ProfileViewModel model);
        string GetCurrentUserId();
        Task<User> GetUser();
    }

}

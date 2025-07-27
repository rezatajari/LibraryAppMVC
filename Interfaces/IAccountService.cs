using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        Task<ResultTask<bool>> LogIn(LoginViewModel model);
        Task<ResultTask<bool>> Registration(RegisterViewModel model);
        Task<ResultTask<bool>> ConfirmationEmailProcess(string userId, string token);
        Task<ResultTask<bool>> DeleteAccount(string email);
        Task<ResultTask<bool>> LogoutAsync();
    }
}

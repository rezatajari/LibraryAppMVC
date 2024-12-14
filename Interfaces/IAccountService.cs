using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        Task<User> Login(LoginViewModel model);
        Task<bool> CheckUserExist(string email, string password);
        Task<bool> Register(RegisterViewModel model);
    }
}

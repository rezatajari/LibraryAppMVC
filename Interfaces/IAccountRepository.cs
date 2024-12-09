using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> Login(LoginViewModel model);
        Task<bool> Register(User newUser);
        Task<bool> CheckUserExist(string email, string password);
    }
}

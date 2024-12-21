using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> Login(LoginViewModel model);
        Task<bool> Register(User newUser);
        Task<bool> CheckUserExist(string email);
        Task<User> GetUserById(int? id);
        Task UpdateUser(User user);
        Task<bool> EmailExists(int? userId, string email);
        Task<bool> RemoveUser(User user);
    }
}

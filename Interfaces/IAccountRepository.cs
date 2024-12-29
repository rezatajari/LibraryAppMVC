using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> GetUserById(int? id);
        Task UpdateUser(User user);
        Task<bool> RemoveUser(User user);
    }
}

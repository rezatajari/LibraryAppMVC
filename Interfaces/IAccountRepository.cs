using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountRepository
    {
        bool Login(LoginViewModel model);
    }
}

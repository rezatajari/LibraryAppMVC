using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        bool Login(LoginViewModel model);
    }
}

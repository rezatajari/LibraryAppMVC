using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;

namespace LibraryAppMVC.Interfaces
{
    public interface IEmailService
    {
        Task<ResultTask<bool>> SendEmail(User user);
    }
}

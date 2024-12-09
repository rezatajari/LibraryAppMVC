using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LibraryDB _libraryDB;
        public AccountRepository(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }

        public bool Login(LoginViewModel model)
        {
            var user = _libraryDB.Users
                .Where(u => u.Email == model.Email && u.Password == model.Password)
                .FirstOrDefault();

            if (user != null)
            {
                return true;
            }

            return false;

        }
    }
}

using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppMVC.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LibraryDB _libraryDB;
        public AccountRepository(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }

        public async Task<User> GetUserById(int? id)
        {
            return await _libraryDB.Users.FindAsync(id);
        }

        public async Task UpdateUser(User user)
        {
            _libraryDB.Users.Update(user);
            await _libraryDB.SaveChangesAsync();
        }


        public async Task<bool> RemoveUser(User user)
        {
            _libraryDB.Users.Remove(user);
            await _libraryDB.SaveChangesAsync();
            return true;
        }
    }
}

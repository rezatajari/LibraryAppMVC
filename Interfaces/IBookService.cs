using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        Task Add(string userId, BookViewModel model);
        Task Remove(Book book);
        Task<List<Book>> GetAll(string userId);
        Task<Book> SearchByTitle(string title, string userId);
    }
}

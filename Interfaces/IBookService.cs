using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        Task Add(int userId, BookViewModel model);
        Task Remove(Book book);
        Task<List<Book>> GetAll(int? userId);
        Task<Book> SearchByTitle(string title);
    }
}
